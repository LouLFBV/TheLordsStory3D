using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class PNJ : InteractableBase
{
    #region Champs/Paramètres
    [Header("Dialogue")]
    [SerializeField] private float distanceToInteract = 2f;
    public string namePNJ;
    public DialogueResponse[] sentences; // Dialogue par défaut
    public DialogueResponse[] sentencesIfPlayerIsBad;
    public bool isOnDial;
    private int index = 0;
    private int sentenceIndex = 0;
    private DialogueResponse[] currentDialogue; // tableau actif
    private bool firstDialoguePlayerDone = false, firstDialoguePnjDone = false;
    private DialogueManager.Speaker currentSpeakerDisplaying;


    [Header("Quêtes")]
    public QuestSO[] questsDisponibles;
    private int currentQuestIndex = 0;
    private QuestSO currentQuestSO;
    [SerializeField] private QuestInstance activeQuestInstance;
    public bool canGiveQuest;
    private bool isPnjInteraction; 
    [SerializeField] private float seuilDeReputationQuest = 0f;


    [Header("Wandering")]
    [SerializeField] private bool canWander = true;
    [SerializeField] private Transform wanderCenter;
    [SerializeField] private float wanderRadius = 6f;
    [SerializeField] private float wanderDelay = 3f;
    private bool canWanderOnStart;
    private float wanderTimer;
    private Vector3 targetPosition;

    private Transform playerTransform;
    private MoveBehaviour moveBehaviour;
    private JumpBehaviour jumpBehaviour;
    private AimBehaviourBasic aimBehaviour;

    private Animator animator;
    private NavMeshAgent agent;
    private float vitesseDeRotation = 0.15f;

    private bool playerIsClose;
    public float dialogueEndTime;
    public float inputCooldown = 1f;
    private float dialogueStartTime;
    private float inputCooldownEnding = 2f;
    #endregion

    private void Start()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        moveBehaviour = playerTransform.GetComponent<MoveBehaviour>();
        jumpBehaviour = playerTransform.GetComponent<JumpBehaviour>();
        aimBehaviour = playerTransform.GetComponent<AimBehaviourBasic>();
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();

        animator.SetBool("isTalking", false);
        wanderTimer = 0f;

        if (wanderCenter == null) wanderCenter = transform;
        canWanderOnStart = canWander;
    }


    public override void OnInteract(PlayerInteractor player)
    {
        Debug.Log("Interacting with PNJ: " + namePNJ);
        if (isOnDial && Time.time - dialogueStartTime > inputCooldown)
        {
            if (!DialogueManager.instance.SkipOrFinish(currentSpeakerDisplaying) && !DialogueManager.instance.inDelay)
                NextLine();
        }
        else if (!firstDialoguePnjDone && !isOnDial && Time.time - dialogueEndTime > inputCooldownEnding)
        {
            StartDialogue();
            SetTargeted(false, playerTransform);
        }
    }
    private void Update()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);
        playerIsClose = distanceToPlayer <= distanceToInteract;

        if (playerIsClose)
        {
            agent.ResetPath();
            animator.SetFloat("Speed", 0f);
        }

        if (canWander && !isOnDial && !playerIsClose)
            Wander();

        animator.SetFloat("Speed", agent.velocity.magnitude);
    }

    private void Wander()
    {
        wanderTimer -= Time.deltaTime;

        if (agent.remainingDistance <= agent.stoppingDistance && !agent.pathPending)
        {
            if (wanderTimer <= 0f)
            {
                Vector3 randomDirection = Random.insideUnitSphere * wanderRadius;
                randomDirection += wanderCenter.position;

                if (NavMesh.SamplePosition(randomDirection, out NavMeshHit hit, wanderRadius, NavMesh.AllAreas))
                {
                    targetPosition = hit.position;
                    agent.SetDestination(targetPosition);
                }

                wanderTimer = wanderDelay;
            }
        }
    }
    #region Start/End Dialogue
    public void StartDialogue()
    {
        if (canGiveQuest)
            ResolveQuestInstance();
        moveBehaviour.StopPlayer();

        StartCoroutine(RotateTowardsToPlayer());
        animator.SetFloat("Speed", 0f);
        canWander = false;
        aimBehaviour.enabled = false;
        jumpBehaviour.canJump = false;
        isOnDial = true;
        agent.isStopped = true;

        DialogueManager.instance.textName.text = namePNJ;
        index = 0;
        dialogueStartTime = Time.time;

        VerifIfIntercationQuest();
        VerifObjectsInInventory();
        AddEnemiesKilled();


        var uiManager = UIManager.instance;
        if (uiManager != null)
        {
            uiManager.HandlePanelOpened();
        }

        if (canGiveQuest && currentQuestSO != null)
        {
            if (activeQuestInstance == null)
            {
                // Nouvelle quête
                currentDialogue = currentQuestSO.sentencesBeforeQuest;
            }
            else
            {
                switch (activeQuestInstance.status)
                {
                    case QuestStatus.InProgress:
                        if (QuestManager.instance.CanCompleteQuest(activeQuestInstance))
                            CompleteQuest();
                        else
                            currentDialogue = currentQuestSO.sentencesQuestInProgress;
                        break;

                    case QuestStatus.Completed:
                        currentDialogue = currentQuestSO.sentencesQuestCompleted;
                        break;
                }
            }
        }
        else
        {
            currentDialogue = sentences;
        }

        // Démarre la lecture
        NextLine();
    }

    private void EndDialogue()
    {
        isOnDial = false;
        index = 0;
        sentenceIndex = 0;
        firstDialoguePlayerDone = false;
        firstDialoguePnjDone = false;
        dialogueEndTime = Time.time;

        if (DialogueManager.instance.dialoguePanel.transform.localScale.y > 0f)
            DialogueManager.instance.ActiveDesactiveDialoguePanel(DialogueManager.instance.animatorDialoguePanel);

        if (DialogueManager.instance.dialoguePlayerPanel.transform.localScale.y > 0f)
            DialogueManager.instance.ActiveDesactiveDialoguePanel(DialogueManager.instance.animatorDialoguePlayerPanel);

        animator.SetBool("isTalking", false);
        if (canWanderOnStart) canWander = true;
        moveBehaviour.StartPlayer();
        jumpBehaviour.canJump = true;
        aimBehaviour.enabled = true;
        agent.isStopped = false;
        if (isPnjInteraction) isPnjInteraction = false;
        var uiManager = UIManager.instance;
        if (uiManager != null)
        {
            uiManager.HandlePanelClosed();
        }
    }
#endregion
    public void NextLine()
    {
        Debug.Log("NextLine called for PNJ: " + namePNJ);
        // Vérifie si le joueur a une mauvaise réputation
        if (PlayerStats.instance.reputationData.reputationPoints < seuilDeReputationQuest
            && sentencesIfPlayerIsBad.Length > 0
            && (activeQuestInstance != null && !currentQuestSO.isMainQuest)
            && currentDialogue != sentencesIfPlayerIsBad)
        {
            currentDialogue = sentencesIfPlayerIsBad;
            index = 0;
            sentenceIndex = 0;
        }

        if (index >= currentDialogue.Length)
        {
            if (activeQuestInstance != null)
            {
                if (activeQuestInstance.data != null)
                {
                    if (activeQuestInstance.status == QuestStatus.Completed && !activeQuestInstance.rewardsGiven)
                    {
                        QuestManager.instance.ApplyRewards(activeQuestInstance);
                        EndDialogue();
                    }
                    else
                        EndDialogue();
                }
                else if (canGiveQuest && currentDialogue == currentQuestSO.sentencesBeforeQuest)
                {
                    DialogueManager.instance.ShowQuestButtons(this);
                    animator.SetBool("isTalking", false);
                }
                else
                    EndDialogue();
            }
            else if (canGiveQuest && currentDialogue == currentQuestSO.sentencesBeforeQuest)
            {
                DialogueManager.instance.ShowQuestButtons(this);
                animator.SetBool("isTalking", false);
            }
            else
                EndDialogue();
            return;
        }

        var dialogueGroup = currentDialogue[index];

        // Affiche le dialogue PNJ ou la réponse du joueur selon l'index
        if (sentenceIndex < dialogueGroup.pnjDialogues.Length)
        {
            currentSpeakerDisplaying = DialogueManager.Speaker.PNJ;
            if (!firstDialoguePnjDone)
            {
                DialogueManager.instance.ShowLine(dialogueGroup.pnjDialogues[sentenceIndex], DialogueManager.Speaker.PNJ, 0.5f);
                firstDialoguePnjDone = true;
                DialogueManager.instance.ActiveDesactiveDialoguePanel(DialogueManager.instance.animatorDialoguePanel);
            }
            else
                DialogueManager.instance.ShowLine(dialogueGroup.pnjDialogues[sentenceIndex], DialogueManager.Speaker.PNJ);
            animator.SetBool("isTalking", true);
        }
        else
        {
            currentSpeakerDisplaying = DialogueManager.Speaker.Player;
            int playerIndex = sentenceIndex - dialogueGroup.pnjDialogues.Length;
            if (!firstDialoguePlayerDone)
            {
                DialogueManager.instance.ShowLine(dialogueGroup.playerResponses[playerIndex], DialogueManager.Speaker.Player, 0.5f);
                firstDialoguePlayerDone = true;
                DialogueManager.instance.ActiveDesactiveDialoguePanel(DialogueManager.instance.animatorDialoguePlayerPanel);
            }
            else
                DialogueManager.instance.ShowLine(dialogueGroup.playerResponses[playerIndex], DialogueManager.Speaker.Player);
            animator.SetBool("isTalking", false);
        }
        sentenceIndex++;

        // Si on a fini toutes les lignes du groupe, passe au groupe suivant
        if (sentenceIndex >= dialogueGroup.pnjDialogues.Length + dialogueGroup.playerResponses.Length)
        {
            sentenceIndex = 0;
            index++;
        }
    }

    #region Accept/Refuse Quest
    public void AcceptQuest()
    {
        QuestManager.instance.AddQuest(currentQuestSO);
        activeQuestInstance = QuestManager.instance.GetQuestInstance(currentQuestSO);

        DialogueManager.instance.HideQuestButtons();

        animator.SetBool("isTalking", true);
        index = 0;
        currentDialogue = currentQuestSO.sentencesQuestAccepted;
        QuestLog.instance.ActiveDesactiveQuestText(currentQuestSO.questName);
        Debug.Log("Quest accepted: " + currentQuestSO.questName);
        NextLine();
    }


    public void RefuseQuest()
    {

        DialogueManager.instance.HideQuestButtons();
        animator.SetBool("isTalking", true);
        index = 0;
        currentDialogue = currentQuestSO.sentencesQuestRefused;
        NextLine();
        activeQuestInstance = null;
    }
    #endregion
    private void CompleteQuest()
    {
        if (activeQuestInstance == null) return;

        QuestManager.instance.CompleteQuest(activeQuestInstance);

        DeleteObjectsInInventory(
            currentQuestSO.requiredItem,
            currentQuestSO.requiredItemCount
        );

        if (currentQuestSO.questName == QuestLog.instance.QuestActiveText.text)
        {
            QuestLog.instance.QuestActiveText.gameObject.SetActive(false);
            QuestLog.instance.questToggle.isOn = false;
        }

        index = 0;
        currentDialogue = currentQuestSO.sentencesQuestCompleted;
        currentQuestIndex++;
        if (currentQuestIndex >= questsDisponibles.Length)
            canGiveQuest = false;
    }


    private void VerifObjectsInInventory()
    {
        if (activeQuestInstance == null || activeQuestInstance.status != QuestStatus.InProgress || currentQuestSO.requiredItem == null) return;
        foreach (var obj in Inventory.instance.GetContent())
        {
            if (currentQuestSO.requiredItem == obj.itemData)
            {
                activeQuestInstance.currentCount += obj.count;
            }
        }
    }

    private void AddEnemiesKilled()
    {
        if (activeQuestInstance == null || activeQuestInstance.status != QuestStatus.InProgress || currentQuestSO.questType != QuestType.Hunt) return;
        foreach (var quest in QuestManager.instance.activeQuests)
        {
            if (quest.data == currentQuestSO)
            {
                activeQuestInstance.currentCount = quest.currentCount;
            }
        }
    }

    private void DeleteObjectsInInventory(ItemData itemData, int count)
    {
        for (int i = 0; i < count; i++)
        {
            Inventory.instance.RemoveItem(itemData);
        }
    }
    private void VerifIfIntercationQuest()
    {
        foreach (var quest in QuestManager.instance.activeQuests)
        {
            if (quest.data.namePNJ == namePNJ && quest.status == QuestStatus.InProgress)
            {
                quest.interactionDone = true;
                activeQuestInstance = quest;
                currentQuestSO = quest.data;
                currentDialogue = currentQuestSO.sentencesInteraction;
                isPnjInteraction = true;
                return;
            }
        }
    }

    private void ResolveQuestInstance()
    {
        activeQuestInstance = null;
        currentQuestSO = null;

        foreach (var questSO in questsDisponibles)
        {
            var instance = QuestManager.instance.GetQuestInstance(questSO);

            // 1️⃣ Quête jamais acceptée
            if (instance == null)
            {
                currentQuestSO = questSO;
                return;
            }

            // 2️⃣ Quête en cours
            if (instance.status == QuestStatus.InProgress)
            {
                activeQuestInstance = instance;
                currentQuestSO = questSO;
                return;
            }

            // 3️⃣ Quête terminée mais récompense pas encore donnée
            if (instance.status == QuestStatus.Completed && !instance.rewardsGiven)
            {
                activeQuestInstance = instance;
                currentQuestSO = questSO;
                return;
            }
        }

        // 4️⃣ Toutes les quêtes sont terminées
        canGiveQuest = false;
    }



    private IEnumerator RotateTowardsToPlayer()
    {
        Vector3 direction = (playerTransform.position - transform.position).normalized;
        direction.y = 0f;

        if (direction != Vector3.zero)
        {
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            float angle = Quaternion.Angle(transform.rotation, lookRotation);

            while (angle > 1f)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * (vitesseDeRotation * 10f));
                angle = Quaternion.Angle(transform.rotation, lookRotation);
                yield return null;
            }
        }
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, distanceToInteract);

        if (wanderCenter != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(wanderCenter.position, wanderRadius);
        }
    }
}
