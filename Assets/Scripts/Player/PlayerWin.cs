using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GD3D.Easing;
using UnityEngine.UI;
using System.Runtime.InteropServices;
namespace GD3D.Player
{
    /// <summary>
    /// 
    /// </summary>
    public class PlayerWin : PlayerScript
    {
        [SerializeField] private Transform winGameMenu;
        private long? _currentMenuPopupEaseID = null;
        [SerializeField] private EaseSettings menuPopupEaseSettings;

        public override void Start()
        {
            base.Start();
            EasingManager.Instance.OnEaseObjectRemove += OnEaseObjectRemove;
        }
        public void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Win"))
            {
                Debug.Log("Win");
                ShowWinMenu();
            }
        }
        public override void Update()
        {
            base.Update();
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
        }
        public void ShowWinMenu()
        {
            ShowMenu(winGameMenu);

        }
        private void ShowMenu(Transform menu)
        {
            EasingManager.TryRemoveEaseObject(_currentMenuPopupEaseID);

            // Create ease object from the menu popup ease settings
            EaseObject easeObj = menuPopupEaseSettings.CreateEase();

            // Set on update to scale the menu
            easeObj.OnUpdate = (obj) =>
            {
                menu.localScale = obj.EaseVector(Vector3.zero, Vector3.one);
            };

            // Set the menu popup ease ID
            _currentMenuPopupEaseID = easeObj.ID;
        }
        private void OnEaseObjectRemove(long easeID)
        {
            // Set the appropriate ease ID to null if that ease ID just got removed
            if (easeID == _currentMenuPopupEaseID)
            {
                _currentMenuPopupEaseID = null;
            }
        }

    }
}
