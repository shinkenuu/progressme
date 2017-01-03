using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Model;

namespace Assets.Scripts.GUI.User
{

    [RequireComponent(typeof(ThumbnailScript))]
    public class UserMenuGui : UserGui, IUserGui
    {
        [SerializeField]
        private ThumbnailScript UserProfileThumbnail;
        [SerializeField]
        private GameObject ManagerOverviewButtonGo;
        [SerializeField]
        private GameObject CreateOppButtonGo;


        protected override void OnEnable()
        {
            UserProfileThumbnail = gameObject.GetComponent<ThumbnailScript>();
            CheckLoggedEmployeeManagerOverviewCapability();
            CheckLoggedEmployeeCreateOppCapability();
        }
        

        private void ShowFocusedEmployeeProfile(int width, int height)
        {
            UserProfileThumbnail.SetImageComponent("Sprites/Employees/" + Watcher.EmployeeInFocus.employee_name + "/profile", height, width);
        }
        

        private void CheckLoggedEmployeeManagerOverviewCapability()
        {
            RepReader.GetEmployeesReportingTo(NetMng.LoggedEmployee.id).Count();

            if (RepReader.GetEmployeesReportingTo(NetMng.LoggedEmployee.id) == null)
            {
                ManagerOverviewButtonGo.SetActive(false);
                return;
            }

            ManagerOverviewButtonGo.SetActive(true);
        }

        
        private void CheckLoggedEmployeeCreateOppCapability()
        {
            if (RepReader.GetDepartment(NetMng.LoggedEmployee.department_id).department_name == "sales")
            {
                CreateOppButtonGo.SetActive(true);
                return;
            }

            CreateOppButtonGo.SetActive(false);
        }


        
        public override void BtnCancel_Click()
        {
        }

        public override void BtnConfirm_Click()
        {
        }
    }
}
