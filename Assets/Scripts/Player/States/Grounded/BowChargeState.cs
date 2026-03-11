using UnityEngine;

public class BowChargeState : GroundedState
{
    private readonly int hHash = Animator.StringToHash("H");
    private readonly int vHash = Animator.StringToHash("V");
    private readonly int speedHash = Animator.StringToHash("Speed");
    private readonly int chargeBool = Animator.StringToHash("ChargeBow");

    private float currentChargeTime;
    private float chargeDuration = 2.067f;

    [Header("Cam Sync Settings")]
    private float aimFOV = 40f; // FOV quand l'arc est tendu au max

    public BowChargeState(PlayerController player) : base(player) { }

    public override void Enter()
    {
        base.Enter();
        currentChargeTime = 0f;
        player.Animator.SetBool(chargeBool, true);
        player.Animator.applyRootMotion = true;

        player.Bow.PrepareArrow();

        // On active l'état de visée sur la caméra au début pour décaler le pivot
        ThirdPersonCameraController.Instance.SetAimState(true);
    }

    public override void Update()
    {
        base.Update();

        // 1. Gestion de la charge (Inchangé)
        if (currentChargeTime < chargeDuration)
        {
            currentChargeTime += Time.deltaTime;
        }
        float t = Mathf.Clamp01(currentChargeTime / chargeDuration);
        player.Bow.UpdateChargeProgress(t);

        // 2. LOGIQUE DE CAMÉRA DYNAMIQUE
        // On veut TOUJOURS le pivot à l'épaule quand on charge l'arc
        Vector3 currentTargetPivot = ThirdPersonCameraController.Instance.AimPivotOffset;

        // Mais on ne veut le rapprochement (CamOffset) QUE si on vise activement
        Vector3 currentTargetCamOffset = player.Input.AimHeld ?
                                         ThirdPersonCameraController.Instance.AimCamOffset :
                                         ThirdPersonCameraController.Instance.DefaultCamOffset;

        // On applique ces valeurs directement à ton contrôleur de caméra
        ThirdPersonCameraController.Instance.SetManualOffsets(currentTargetPivot, currentTargetCamOffset);

        // FOV : Zoom progressif seulement si on vise ? 
        // Ou zoom léger constant ? Ici, zoom progressif uniquement si AimPressed
        if (player.Input.AimHeld)
        {
            float dynamicFOV = Mathf.Lerp(ThirdPersonCameraController.Instance.DefaultFOV, 40f, t);
            ThirdPersonCameraController.Instance.SetFOV(dynamicFOV);
        }
        else
        {
            ThirdPersonCameraController.Instance.ResetFOV();
        }

        // 3. Rotation et Tir (Inchangé)
        RotateTowardsCamera();

        // 4. Mouvement & Animation
        Vector2 input = player.Input.MoveInput;
        player.Animator.SetFloat(hHash, input.x, 0.1f, Time.deltaTime);
        player.Animator.SetFloat(vHash, input.y, 0.1f, Time.deltaTime);

        float moveSpeedFactor = Mathf.Lerp(1f, 0.5f, t);
        player.Animator.SetFloat(speedHash, input.magnitude * moveSpeedFactor, 0.1f, Time.deltaTime);

        // 5. Condition de tir (Ce qui était dans "HandleShooting")
        if (!player.Input.AttackPressed)
        {
            if (player.Bow.canShoot)
            {
                player.Bow.ShootArrow();
                player.StateMachine.ChangeState(PlayerStateType.Idle);
            }
        }
    }

    public override void Exit()
    {
        base.Exit();
        player.Animator.SetBool(chargeBool, false);
        player.Animator.SetFloat(speedHash, 0f);

        // Reset de la caméra
        ThirdPersonCameraController.Instance.SetAimState(false);
        ThirdPersonCameraController.Instance.ResetFOV();
    }

    private void RotateTowardsCamera()
    {
        Vector3 camForward = Camera.main.transform.forward;
        camForward.y = 0;
        if (camForward != Vector3.zero)
        {
            player.transform.rotation = Quaternion.Slerp(
                player.transform.rotation,
                Quaternion.LookRotation(camForward),
                10f * Time.deltaTime);
        }
    }
}