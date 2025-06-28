using UnityEngine;

public class LocalInputs : MonoBehaviour
{
    private NetworkInputData _networkInputData;

    private bool _isJumpPressed;
    private bool _isFirePressed;
    
    void Start()
    {
        _networkInputData = new NetworkInputData();
    }

    void Update()
    {
        _networkInputData.horizontalMovementInput = Input.GetAxis("Horizontal");
        _networkInputData.verticalMovementInput = Input.GetAxis("Vertical");

        if (Input.GetKeyDown(KeyCode.F))
        {
            _isFirePressed = true;
        }

        //_isFirePressed |= Input.GetKeyDown(KeyCode.Space);
        
        _isJumpPressed |= Input.GetKeyDown(KeyCode.Space);
    }

    public NetworkInputData GetLocalInputs()
    {
        _networkInputData.isFirePressed = _isFirePressed;
        _isFirePressed = false;

        _networkInputData.networkButtons.Set(MyButtons.Jump, _isJumpPressed);
        _isJumpPressed = false;
        
        return _networkInputData;
    }
}
