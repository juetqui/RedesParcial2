using Fusion;

public struct NetworkInputData : INetworkInput
{
    public float horizontalMovementInput;
    public float verticalMovementInput;
    public NetworkBool isFirePressed;

    public NetworkButtons networkButtons;
}

enum MyButtons
{
    Jump = 0,
}
