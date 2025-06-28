using UnityEngine;

public class LocalInputs : MonoBehaviour
{
    private NetworkInputData _networkInputData;

    private bool _isJumpPressed;
    private bool _isFirePressed;
    private bool _isFireSecondaryPressed;

    void Start()
    {
        _networkInputData = new NetworkInputData();
    }

    void Update()
    {
        _networkInputData.horizontalMovementInput = Input.GetAxis("Horizontal");
        _networkInputData.verticalMovementInput = Input.GetAxis("Vertical");

        if (Input.GetKey(KeyCode.Mouse0))
        {
            _isFirePressed = true;
        }

        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            _isFireSecondaryPressed = true;
        }

        //_isFirePressed |= Input.GetKeyDown(KeyCode.Space);

        _isJumpPressed |= Input.GetKey(KeyCode.Space);
    }

    public NetworkInputData GetLocalInputs()
    {
        _networkInputData.isFirePressed = _isFirePressed;
        _isFirePressed = false;

        _networkInputData.isFireSecondaryPressed = _isFireSecondaryPressed;
        _isFireSecondaryPressed = false;

        _networkInputData.networkButtons.Set(MyButtons.Jump, _isJumpPressed);
        _isJumpPressed = false;
        
        return _networkInputData;
    }
}
