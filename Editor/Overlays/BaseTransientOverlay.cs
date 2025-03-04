using UnityEditor.Overlays;
using UnityEngine.UIElements;

namespace IKGTools.SplineBones.Editor
{
    internal abstract class BaseTransientOverlay : Overlay, ITransientOverlay
    {
        private bool _isInitialized = false;
        private bool _isVisible = false;

        public bool IsInitialized => _isInitialized;
        
        public bool visible => IsVisible();

        public virtual bool IsVisible() => _isVisible;

        public void SetVisible(bool visible)
        {
            _isVisible = visible;
        }
        
        protected abstract VisualElement CreateUIElements();
        protected virtual void InitializeProcess(){}

        protected virtual void OnBeforeShow(){}
        protected virtual void OnBeforeHide(){}
        protected virtual void OnShown(){}
        protected virtual void OnHidden(){}
        protected virtual void SubscribeToEvents(){}
        protected virtual void UnsubscribeFromEvents(){}

        
        public override VisualElement CreatePanelContent()
        {
            return CreateUIElements();
        }
        

        public override void OnCreated()
        {
            base.OnCreated();
            displayedChanged += OnDisplayedChanged;
        }

        public override void OnWillBeDestroyed()
        {
            base.OnWillBeDestroyed();
            displayedChanged -= OnDisplayedChanged;
        }

        private void OnDisplayedChanged(bool isDisplayed)
        {
            if (isDisplayed)
                OnShow();
            else
                OnHide();
        }
        
        private void OnShow()
        {
            if (!_isInitialized)
                Initialize();

            OnBeforeShow();
            
            SubscribeToEvents();

            OnShown();
        }

        private void Initialize()
        {
            InitializeProcess();
            _isInitialized = true;
        }
        
        private void OnHide()
        {
            OnBeforeHide();
            UnsubscribeFromEvents();
            OnHidden();
        }
    }
}