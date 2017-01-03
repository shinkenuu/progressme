using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Model;
using Assets.Scripts.GUI.User;

namespace Assets.Scripts.GUI.User.Detail.Opportunity
{

    public class OppDetailsGUIScript : UserGui, IUserGui
    {

        //private readonly string _accountFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\ProgressMe\Accounts\";

        //#region References

        //[SerializeField]
        //private Text msgTxt;

        //[SerializeField]
        //private Image responsableProfileImage;
        //[SerializeField]
        //private Text responsableNameText;
        //[SerializeField]
        //private Image accountLogoImage;
        //[SerializeField]
        //private Text accountNameText;

        //[SerializeField]
        //private Text taskNameTxt;
        //[SerializeField]
        //private Text currentProcessTxt;
        //[SerializeField]
        //private Text taskNoteTxt;

        //[SerializeField]
        //private Transform procedurePanelTransform;
        //[SerializeField]
        //private Transform procedureOptionPrefab;

        //[SerializeField]
        //private Transform processPanelTransform;
        //[SerializeField]
        //private Transform processOptionPrefab;

        //[SerializeField]
        //private GameObject btnNextGo;

        //#endregion

        //private OPPORTUNITY _detailedOpp;
        //private IEnumerable<OPPORTUNITY_HISTORY> _oppHistories;
        //private GameManagerScript.Procedures _selectedProcedure;
        //private PROCESS _selectedProcess;

        //protected override void OnEnable()
        //{
        //    base.OnEnable();
        //    btnNextGo.SetActive(false);
        //    ClearPanel(processPanelTransform);
        //    msgTxt.text = "";
        //}


        //#region Opp info

        //private IEnumerator FillInOppInfo()
        //{
        //    yield return new WaitWhile(() => (GameMng == null));

        //    _detailedOpp = GameMng._tempOpp;
        //    _oppHistories = GameMng._tempOppHistories;

        //    yield return new WaitWhile(() => (RepReader == null));

        //    EMPLOYEE responsableWorkerProfile = RepReader.GetEmployee(_detailedOpp.responsable_employee_id ?? 0);

        //    if (responsableWorkerProfile != null)
        //    {
        //        // Responsable Worker Profile
        //        responsableProfileImage.sprite = Sprite.Create((Texture2D)Resources.Load("Sprites/Employees/" + responsableWorkerProfile.employee_name + "/profile"), new Rect(0, 0, 50, 50), new Vector2(0.5f, 0.5f));
        //        //Responsable Worker Name
        //        responsableNameText.text = responsableWorkerProfile.employee_name.Replace('.', ' ');
        //    }

        //    else
        //    {
        //        //Responsable Worker Profile Photo
        //        responsableProfileImage.sprite = null;
        //        //Responsable Worker Name
        //        responsableNameText.text = "";
        //    }

        //    //Opp Name
        //    taskNameTxt.text = _detailedOpp.opportunity_name;
        //    //Opp current process
        //    currentProcessTxt.text = RepReader.GetProcess(_detailedOpp.current_process_id ?? 0).description;
        //    //Opp Note
        //    taskNoteTxt.text = _detailedOpp.note;

        //    //account
        //    ACCOUNT account = RepReader.GetAccount(_detailedOpp.account_id);
        //    accountNameText.text = account.account_name;

        //    WWW www = new WWW("file:///" + _accountFolder.Replace('\\', '/') + account.account_name + "/logo.png");

        //    if (www.texture.height == 100 && www.texture.width == 100)
        //    {
        //        accountLogoImage.sprite = Sprite.Create(www.texture, new Rect(0, 0, www.texture.height, www.texture.width), new Vector2(0.5f, 0.5f));
        //    }

        //    else
        //    {
        //        accountLogoImage.gameObject.SetActive(false);
        //    }

        //    //If there is history of this task 
        //    if (_oppHistories != null && _oppHistories.Count() > 0)
        //    {
        //        DisplayPossibleProcedures(_oppHistories.First());
        //    }

        //    scheduledActions.RemoveAt(0);
        //    FetchNextAction();
        //    yield break;
        //}

        //#endregion





        //private void DisplayPossibleProcedures(OPPORTUNITY_HISTORY firstOppHistory)
        //{
        //    ClearPanel(procedurePanelTransform);

        //    if (firstOppHistory == null)
        //    {
        //        Debug.LogError("::CRITICAL:: Attempt to DisplayProcedureOptions for a task without providing the firstOppHistory");
        //        return;
        //    }

        //    GameObject procedureOptionGo;

        //    //Able to advance
        //    procedureOptionGo = Instantiate(procedureOptionPrefab).gameObject;
        //    procedureOptionGo.GetComponent<Image>().sprite = Resources.Load("Sprites/GUI/advance_opp_icon", typeof(Sprite)) as Sprite;
        //    procedureOptionGo.GetComponent<Button>().onClick.AddListener(() => SetProcedure(GameManagerScript.Procedures.OppAdvance));
        //    procedureOptionGo.transform.SetParent(procedurePanelTransform, false);

        //    //Able to Transfer task to another worker
        //    if (RepReader.GetEmployeesIn(RepReader.GetProcess(_detailedOpp.current_process_id ?? 0).department_id) != null)
        //    {
        //        procedureOptionGo = Instantiate(procedureOptionPrefab).gameObject;
        //        procedureOptionGo.GetComponent<Image>().sprite = Resources.Load("Sprites/GUI/transfer_opp_icon", typeof(Sprite)) as Sprite;
        //        procedureOptionGo.GetComponent<Button>().onClick.AddListener(() => SetProcedure(GameManagerScript.Procedures.OppTransfer));
        //        procedureOptionGo.transform.SetParent(procedurePanelTransform, false);
        //    }

        //    //End task
        //    procedureOptionGo = Instantiate(procedureOptionPrefab).gameObject;
        //    procedureOptionGo.GetComponent<Image>().sprite = Resources.Load("Sprites/GUI/end_opp_icon", typeof(Sprite)) as Sprite;
        //    procedureOptionGo.GetComponent<Button>().onClick.AddListener(() => SetProcedure(GameManagerScript.Procedures.OppEnd));
        //    procedureOptionGo.transform.SetParent(procedurePanelTransform, false);
        //}

        //private void DisplayAdjacentProcesses()
        //{
        //    ClearPanel(processPanelTransform);

        //    IEnumerable<PROCESS> adjacentProcesses = RepReader.GetNextProcessesOf(RepReader.GetProcess(_detailedOpp.current_process_id ?? 0).id);

        //    if (adjacentProcesses == null)
        //    {
        //        msgTxt.text = "Unable to find process adjacent to process '" + RepReader.GetProcess(_detailedOpp.current_process_id ?? 0).process_name + "'";
        //        return;
        //    }

        //    foreach (PROCESS adjacentProcess in adjacentProcesses)
        //    {
        //        GameObject processOptionGo = Instantiate(processOptionPrefab).gameObject;
        //        processOptionGo.GetComponent<Text>().text = adjacentProcess.process_name;
        //        processOptionGo.GetComponent<Button>().onClick.AddListener(() => SetProcess(processOptionGo.GetComponent<Text>().text));
        //        processOptionGo.transform.SetParent(processPanelTransform, false);
        //    }

        //}


        //private void SetProcess(string processName)
        //{
        //    _selectedProcess = RepReader.GetProcess(processName);
        //    btnNextGo.SetActive(true);

        //    if (_selectedProcedure == GameManagerScript.Procedures.OppAdvance)
        //    {
        //        msgTxt.text = "Advance to '" + _selectedProcess.description + "' ?";
        //    }
        //    else
        //    {
        //        msgTxt.text = "Retreat to '" + _selectedProcess.description + "' ?";
        //    }
        public override void BtnCancel_Click()
        {
            throw new NotImplementedException();
        }

        public override void BtnConfirm_Click()
        {
            throw new NotImplementedException();
        }
    }
}

