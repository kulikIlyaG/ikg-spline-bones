using Editor.Services.State.States;
using IKGTools.SplineBones.Editor.EasyStateMachine;

namespace IKGTools.Editor.Services.States
{
    public abstract class BaseEditEditorState : EditorEasyState
    {
        protected BaseEditEditorState(IStateMachine stateMachine) : base(stateMachine)
        {
        }
    }
}