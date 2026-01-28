using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private Transform cameraRoot;
    [SerializeField] private GameManager gameManager;

    public SkillManager Skills { get; private set; }

    private CharacterController _controller;
    private PlayerInputActions _input;
    private Vector2 _moveInput;
    private float _speed;
    private bool _inputEnabled = true;
    private ThirdPersonCamera _camera;

    private void Awake()
    {
        _controller = GetComponent<CharacterController>();
        Skills = gameObject.AddComponent<SkillManager>();
        _camera = cameraRoot ? cameraRoot.GetComponent<ThirdPersonCamera>() : null;

        _input = new PlayerInputActions();
        _input.Player.Move.performed += ctx => _moveInput = ctx.ReadValue<Vector2>();
        _input.Player.Move.canceled += _ => _moveInput = Vector2.zero;
        _input.Player.Look.performed += OnLook;

        if (!gameManager)
            gameManager = FindFirstObjectByType<GameManager>();

        if (!gameManager)
        {
            Debug.LogError($"{nameof(PlayerController)}: {nameof(GameManager)} not found.", this);
            enabled = false;
            return;
        }

        gameManager.OnLevelUp += OnLevelUp;
        gameManager.RegisterPlayer(this);
    }

    private void OnEnable() => _input.Enable();
    private void OnDisable() => _input.Disable();
    private void OnDestroy()
    {
        if (gameManager)
            gameManager.OnLevelUp -= OnLevelUp;
    }

    private void Start()
    {
        _speed = gameManager.Config.player.moveSpeed;
        Skills.AddSkill(SkillType.Projectile);
    }

    private void Update()
    {
        if (!_inputEnabled)
            return;

        Move();
        Skills.Tick();
    }

    public void SetInput(bool value)
    {
        _inputEnabled = value;
    }

    private void Move()
    {
        if (_moveInput.sqrMagnitude < 0.01f)
            return;

        var camForward = cameraRoot.forward;
        camForward.y = 0;
        camForward.Normalize();

        var camRight = cameraRoot.right;
        var moveDir = camForward * _moveInput.y + camRight * _moveInput.x;

        _controller.Move(moveDir * _speed * Time.deltaTime);

        RotatePlayer(moveDir);
    }

    private void RotatePlayer(Vector3 direction)
    {
        var targetRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 10f * Time.deltaTime);
    }

    private void OnLook(UnityEngine.InputSystem.InputAction.CallbackContext ctx)
    {
        var delta = ctx.ReadValue<Vector2>();
        if (_camera)
            _camera.AddInput(delta);
    }

    private void OnLevelUp()
    {
        if (gameManager.LevelUpUI)
            gameManager.LevelUpUI.Show(this);
    }
}