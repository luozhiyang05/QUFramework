using System;
using Framework;
using UnityEngine;

namespace GameSystem.MVCTemplate
{
    [RequireComponent(typeof(CanvasGroup))]
    public abstract class BaseView : MonoBehaviour, IController
    {
        public CanvasGroup CanvasGroup;
        public bool isOpen;
        protected BaseModel Model;
        
        public virtual void Awake()
        {
            CanvasGroup = GetComponent<CanvasGroup>();
            
        }

        private void OnEnable()
        {
            OnShow();
        }

        private void Start()
        {
            SetModelCallback();
            InitUI();
        }

        private void OnDisable()
        {
            OnHide();
        }
        
        public void SetModel(BaseModel model)
        {
            Model = model;
        }

        protected abstract void SetModelCallback();

        protected abstract void InitUI();

        protected virtual void OnShow()
        {
            Model.BindListener();
        }

        protected virtual void OnHide()
        {
            Model.RemoveListener();
        }

        public IMgr Ins => Global.GetInstance();
    }
}