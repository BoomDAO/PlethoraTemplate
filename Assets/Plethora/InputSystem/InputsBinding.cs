namespace PlethoraV2.Input
{
    using UnityEngine;


    public class InputsBinding : MonoBehaviour
    {
        private Inputs playerInput;

        protected void Awake()
        {
            //Create new Instance of player input
            playerInput = new Inputs();

            ////Vector2

            //PERFORM
            playerInput.Main.Movement.performed += ctx => MovementHandler(ctx.ReadValue<Vector2>());
            playerInput.Main.Look.performed += ctx => LookHandler(ctx.ReadValue<Vector2>());
            playerInput.Main.MousePos.performed += ctx => MousePosHandler(ctx.ReadValue<Vector2>());
            playerInput.Main.Scroll.performed += ctx => ScrollHandler(ctx.ReadValue<float>());

            //CANCEL
            playerInput.Main.Movement.canceled += ctx => MovementHandler(ctx.ReadValue<Vector2>());
            playerInput.Main.Look.canceled += ctx => LookHandler(ctx.ReadValue<Vector2>());
            playerInput.Main.MousePos.canceled += ctx => MousePosHandler(ctx.ReadValue<Vector2>());
            playerInput.Main.Scroll.canceled += ctx => ScrollHandler(ctx.ReadValue<float>());


            ////Buttons

            //PERFORM
            playerInput.Main.Any.performed += ctx => AnyHandler(ctx.ReadValueAsButton());
            playerInput.Main.Jump.performed += ctx => JumpHandler(ctx.ReadValueAsButton());
            playerInput.Main.InteractLeft.performed += ctx => InteractLeft(ctx.ReadValueAsButton());
            playerInput.Main.InteractRight.performed += ctx => InteractRight(ctx.ReadValueAsButton());
            playerInput.Main.Crouch.performed += ctx => CrouchHandler(ctx.ReadValueAsButton());
            playerInput.Main.Sprint.performed += ctx => SprintHandler(ctx.ReadValueAsButton());
            playerInput.Main.Inventory.performed += ctx => InventoryHandler(ctx.ReadValueAsButton());
            playerInput.Main.Pickup.performed += ctx => PickupHandler(ctx.ReadValueAsButton());
            playerInput.Main.Menu.performed += ctx => MenuHandler(ctx.ReadValueAsButton());
            playerInput.Main.Dance.performed += ctx => DanceHandler(ctx.ReadValueAsButton());

            //CANCEL
            playerInput.Main.Any.canceled += ctx => AnyHandler(ctx.ReadValueAsButton());
            playerInput.Main.Jump.canceled += ctx => JumpHandler(ctx.ReadValueAsButton());
            playerInput.Main.InteractLeft.canceled += ctx => InteractLeft(ctx.ReadValueAsButton());
            playerInput.Main.InteractRight.canceled += ctx => InteractRight(ctx.ReadValueAsButton());
            playerInput.Main.Crouch.canceled += ctx => CrouchHandler(ctx.ReadValueAsButton());
            playerInput.Main.Sprint.canceled += ctx => SprintHandler(ctx.ReadValueAsButton());
            playerInput.Main.Inventory.canceled += ctx => InventoryHandler(ctx.ReadValueAsButton());
            playerInput.Main.Pickup.canceled += ctx => PickupHandler(ctx.ReadValueAsButton());
            playerInput.Main.Menu.canceled += ctx => MenuHandler(ctx.ReadValueAsButton());
            playerInput.Main.Dance.canceled += ctx => DanceHandler(ctx.ReadValueAsButton());
        }

        private void MousePosHandler(Vector2 vector2)
        {
            //InputMaster.CamMousePos = vector2;
            //InputMaster.WorldMousePos = vector2;
        }

        private void ScrollHandler(float v)
        {
            InputMaster.Scroll = v;
        }
        private void MovementHandler(Vector2 vector2)
        {
            InputMaster.Movement = vector2;
        }

        private void LookHandler(Vector2 vector2)
        {
            InputMaster.Look = vector2;
        }
        private void AnyHandler(bool value)
        {
            InputMaster.PressButton(InputMaster.InputType.Any, value);
        }
        private void JumpHandler(bool value)
        {
            InputMaster.PressButton(InputMaster.InputType.Jump, value);
        }

        private void InteractLeft(bool value)
        {
            InputMaster.PressButton(InputMaster.InputType.InteractLeft, value);
        }

        private void InteractRight(bool value)
        {
            InputMaster.PressButton(InputMaster.InputType.InteractRight, value);
        }

        private void CrouchHandler(bool value)
        {
            InputMaster.PressButton(InputMaster.InputType.Crouch, value);
        }

        private void SprintHandler(bool value)
        {
            InputMaster.PressButton(InputMaster.InputType.Sprint, value);
        }

        private void InventoryHandler(bool value)
        {
            InputMaster.PressButton(InputMaster.InputType.Inventory, value);
        }

        private void PickupHandler(bool value)
        {
            InputMaster.PressButton(InputMaster.InputType.Pickup, value);
        }

        private void MenuHandler(bool value)
        {
            InputMaster.PressButton(InputMaster.InputType.Menu, value);
        }
        private void DanceHandler(bool value)
        {
            InputMaster.PressButton(InputMaster.InputType.Dance, value);
        }


        private void OnEnable()
        {
            playerInput.Main.Enable();

        }

        private void OnDisable()
        {
            playerInput.Main.Disable();
        }
    }
}