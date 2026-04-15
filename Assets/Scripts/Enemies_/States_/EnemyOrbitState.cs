using UnityEngine;

public class EnemyOrbitState : EnemyState
{
    private float _directionTimer;
    private int _orbitDirection = 1; // 1 pour droite, -1 pour gauche
    private float _attackTimer = 3f;

    public EnemyOrbitState(EnemyController enemy) : base(enemy) { }

    public override void Enter()
    {
        Debug.Log($"[{enemy.name}] Entering Orbit State");
        enemy.Animator.SetBool("isStrafing", true);

        // ON FORCE LE MOUVEMENT : On met la distance d'arrêt à 0 
        // pour que l'agent cherche toujours à bouger vers le point latéral
        agent.stoppingDistance = 0f;

        agent.updateRotation = false;
        _attackTimer = Random.Range(2f, 5f);
        _directionTimer = Random.Range(2f, 4f);
    }

    public override void Update()
    {
        if (enemy.target == null) return;

        float distance = Vector3.Distance(enemy.transform.position, enemy.target.position);

        // 1. Si le joueur s'éloigne trop, on repasse en Follow
        if (distance > enemy.enemyData.visionRange)
        {
            enemy.StateMachine.ChangeState(EnemyStateType.Follow);
            return;
        }

        // 2. Si on est à portée d'attaque, on a une chance de frapper
        if (enemy.PeekBestAttack() != null)
        {
            enemy.StateMachine.ChangeState(EnemyStateType.Attack);
            return;
        }

        _attackTimer -= Time.deltaTime;
        if (_attackTimer <= 0)
        {
            enemy.StateMachine.ChangeState(EnemyStateType.Follow);
            return;
        }

        // 3. Logique de déplacement orbital (Strafing)
        OrbitMovement();

        // Update Animation
        enemy.Animator.SetFloat("Speed", _orbitDirection, 0.1f, Time.deltaTime);
    }

    private void OrbitMovement()
    {
        // 1. Gestion du changement de direction (inchangé)
        _directionTimer -= Time.deltaTime;
        if (_directionTimer <= 0)
        {
            _orbitDirection *= -1;
            _directionTimer = Random.Range(2f, 5f);
        }

        // 2. Calcul du vecteur latéral (le "Strafe")
        // On prend la direction vers le joueur et on la tourne de 90 degrés
        Vector3 directionToPlayer = (enemy.target.position - enemy.transform.position).normalized;
        Vector3 sideDirection = Vector3.Cross(directionToPlayer, Vector3.up).normalized * _orbitDirection;

        // 3. On ajoute une force pour maintenir la distance (pour ne pas qu'il s'éloigne ou s'approche trop)
        float currentDistance = Vector3.Distance(enemy.transform.position, enemy.target.position);
        float distanceError = currentDistance - enemy.AIManager.OrbitDistance;
        Vector3 forwardCorrection = directionToPlayer * distanceError;

        // 4. Calcul de la vitesse finale
        Vector3 desiredVelocity = (sideDirection + forwardCorrection).normalized * (enemy.AIManager.GetData().orbitSpeedMultiplier);

        // 5. DEPLACEMENT DIRECT (Le secret est là)
        // On utilise Move au lieu de SetDestination
        agent.Move(desiredVelocity * Time.deltaTime);

        // 6. ROTATION (inchangée, l'ours fait face au joueur)
        Vector3 lookDir = directionToPlayer;
        lookDir.y = 0;
        if (lookDir != Vector3.zero)
        {
            enemy.transform.rotation = Quaternion.Slerp(enemy.transform.rotation, Quaternion.LookRotation(lookDir), Time.deltaTime * 5f);
        }
    }
    public override void Exit()
    {
        enemy.Animator.SetBool("isStrafing", false);
        enemy.Animator.SetFloat("Speed", 0);
        agent.updateRotation = true; // On redonne le contrôle à l'agent
        // On empêche de ré-entrer en orbite pendant 4 secondes
        enemy.AIManager.StartOrbitCooldown(4f);
    }
}