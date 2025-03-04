using IKGTools.Editor.EasyStateMachine;

namespace IKGTools.SplineBones.Editor.Services.States
{
    internal abstract class BaseEditEditorState : EditorEasyState
    {
        protected BaseEditEditorState(IStateMachine stateMachine) : base(stateMachine)
        {
        }
    }
}