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
    public QuestInstance[] questsDisponibles;
    private int currentQuestIndex = 0;
    private QuestInstance activeQuest;        // Quête actuellement discutée
    public bool canGiveQuest;
    private bool isPnjInteraction; 
    private int intPnjInteraction = 0;
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

        // Choix du dialogue initial
        if (canGiveQuest)
        {
            if (activeQuest != null && !isPnjInteraction)
            {
                if (activeQuest.status == QuestStatus.NotStarted)
                    currentDialogue = activeQuest.data.sentencesBeforeQuest;
                else if (activeQuest.status == QuestStatus.InProgress)
                    if (QuestManager.instance.CanCompleteQuest(activeQuest))
                        CompleteQuest();
                    else
                        currentDialogue = activeQuest.data.sentencesQuestInProgress;
                else
                    currentDialogue = activeQuest.data.sentencesQuestCompleted;
            }
            else if (questsDisponibles != null && currentQuestIndex < questsDisponibles.Length)
            {
                activeQuest = questsDisponibles[currentQuestIndex];
                currentDialogue = activeQuest.data.sentencesBeforeQuest;
            }
        }
        else if (!isPnjInteraction)
        {
            currentDialogue = sentences;
            activeQuest = null;
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
    }
#endregion
    public void NextLine()
    {
        // Vérifie si le joueur a une mauvaise réputation
        if (PlayerStats.instance.reputationData.reputationPoints < seuilDeReputationQuest
            && sentencesIfPlayerIsBad.Length > 0
            && (activeQuest != null && !activeQuest.data.isMainQuest)
            && currentDialogue != sentencesIfPlayerIsBad)
        {
            currentDialogue = sentencesIfPlayerIsBad;
            index = 0;
            sentenceIndex = 0;
        }

        if (index >= currentDialogue.Length)
        {
            // Fin du dialogue
            if(activeQuest != null)
            {
                if (activeQuest.status == QuestStatus.NotStarted && currentDialogue != sentencesIfPlayerIsBad)
                {
                    DialogueManager.instance.ShowQuestButtons(this);
                    animator.SetBool("isTalking", false);
                }
                else if(activeQuest.status == QuestStatus.Completed)
                {
                    QuestManager.instance.ApplyRewards(activeQuest.data.rewards);
                    activeQuest = null;
                }
                else
                    EndDialogue();
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
        if (activeQuest == null) return;

        QuestManager.instance.AddQuest(activeQuest.data);
        activeQuest.status = QuestStatus.InProgress;
        DialogueManager.instance.HideQuestButtons();

        animator.SetBool("isTalking", true);
        index = 0;
        currentDialogue = activeQuest.data.sentencesQuestAccepted;
        NextLine();
    }

    public void RefuseQuest()
    {
        if (activeQuest == null) return;

        DialogueManager.instance.HideQuestButtons();
        animator.SetBool("isTalking", true);
        index = 0;
        currentDialogue = activeQuest.data.sentencesQuestRefused;
        NextLine();
        activeQuest = null;
    }
    #endregion
    private void CompleteQuest()
    {
        if (activeQuest == null) return;

        activeQuest.status = QuestStatus.Completed;
        if (activeQuest.data.questName == QuestLog.instance.QuestActiveText.text)
        {
            QuestLog.instance.QuestActiveText.gameObject.SetActive(false);
            QuestLog.instance.questToggle.isOn = false;
        }
        QuestManager.instance.CompleteQuest(activeQuest);
        DeleteObjectsInInventory(activeQuest.data.requiredItem, activeQuest.data.requiredItemCount);

        index = 0;
        currentDialogue = activeQuest.data.sentencesQuestCompleted;

        currentQuestIndex++; // Passe à la quête suivante
    }

    private void VerifObjectsInInventory()
    {
        if (activeQuest == null || activeQuest.status != QuestStatus.InProgress || activeQuest.data.requiredItem == null) return;
        foreach (var obj in Inventory.instance.GetContent())
        {
            if (activeQuest.data.requiredItem == obj.itemData)
            {
                activeQuest.currentCount += obj.count;
            }
        }
    }

    private void AddEnemiesKilled()
    {
        if (activeQuest == null || activeQuest.status != QuestStatus.InProgress || activeQuest.data.questType != QuestType.Hunt) return;
        foreach (var quest in QuestManager.instance.activeQuests)
        {
            if (quest.data == activeQuest.data)
            {
                activeQuest.currentCount = quest.currentCount;
            }
        }
    }

    private void DeleteObjectsInInventory(ItemData itemData, int count)
    {
        for (int i = 0; i < activeQuest.data.requiredItemCount; i++)
        {
            Inventory.instance.RemoveItem(itemData);
        }
    }
    private void VerifIfIntercationQuest()
    {
        foreach(var quest in QuestManager.instance.activeQuests)
        {
            if (quest.data.namePNJ == namePNJ && quest.status == QuestStatus.InProgress)
            {
                quest.interactionDone = true;
                currentDialogue = quest.data.sentencesInteraction;
                if(intPnjInteraction == 0)
                    isPnjInteraction = true;
                intPnjInteraction++;
                return;
            }
        }
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
