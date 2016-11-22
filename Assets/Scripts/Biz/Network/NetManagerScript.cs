using UnityEngine;
using UnityEngine.Networking;
using System;
using System.IO;
using Assets.Scripts;
using Assets.Scripts.Model;

[RequireComponent((typeof(GameManagerScript)))]
public class NetManagerScript : NetworkManager {

    private readonly string _documentsFolderPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\ProgressMe\";
    private string serverIp = "127.0.0.1"; // Changed after ReadServerIPFromTextFile
    private readonly int port = 7777;

    private GameManagerScript GameMng;

    private EMPLOYEE _loggedEmployee;
    private int _curConnectionId;

    private void OnEnable()
    {
        GameMng = gameObject.GetComponent<GameManagerScript>();
        ReadServerIPFromTextFile();
    }

    #region Server

    public void StartServerSafely()
    {
        StopClient();
        while(isNetworkActive)
        {
            //wait for the client to stop
        }

        Debug.Log("Starting server...");
        base.StartServer();
    }

    public override void OnStartServer()
	{
		Debug.Log("Server started.");
        base.OnStartServer();

	}

    public override void OnServerConnect(NetworkConnection conn)
    {
        GameMng.LogIntoServerDisplay("A client has connected to the server. ConnId: " + conn.connectionId.ToString());
        Debug.Log("A client has connected to the server. ConnId: " + conn.connectionId.ToString());
        base.OnServerConnect(conn);
    }

    public override void OnServerDisconnect(NetworkConnection conn)
	{
        GameMng.LogIntoServerDisplay("A client has disconnected from the server. ConnId: " + conn.connectionId.ToString());
        Debug.Log("A client has disconnected from the server. ConnId: " + conn.connectionId.ToString());
        base.OnServerDisconnect(conn);

        //NetworkServer.DestroyPlayersForConnection(conn);
        GameMng.LogIntoServerDisplay("ConnId " + conn.connectionId + " disconnected from server. It should destroy the playerObj of this conn.");
        Debug.Log("ConnId " + conn.connectionId + " disconnected from server. It should destroy the playerObj of this conn.");
    }

    public override void OnServerReady(NetworkConnection conn)
    {
        GameMng.LogIntoServerDisplay("Client ready on conn " + conn.connectionId.ToString());
        Debug.Log("Client ready on conn " + conn.connectionId.ToString());
        base.OnServerReady(conn);     
    }

    public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId)
	{
        GameObject playerGo;

		playerGo = (GameObject)Instantiate(playerPrefab, Vector3.zero, Quaternion.identity) as GameObject;
		playerGo.name = "PlayerGO_" + conn.connectionId;
        NetworkServer.AddPlayerForConnection(conn, playerGo, playerControllerId);

        GameMng.LogIntoServerDisplay("Player added with conn " + conn.connectionId.ToString() + " and playerCntrlId " + playerControllerId.ToString());
        Debug.Log("Player added with conn " + conn.connectionId.ToString() + " and playerCntrlId " + playerControllerId.ToString());
    }

    public override void OnServerRemovePlayer(NetworkConnection conn, PlayerController player)
    {
		if (player.IsValid)
        {
            if (player.unetView != null && player.unetView.gameObject != null)
                NetworkServer.Destroy(player.unetView.gameObject);
        }

        GameMng.LogIntoServerDisplay("Player removed with connId " + conn.connectionId.ToString() + " and playerCntrlId " + player.playerControllerId.ToString());
        Debug.Log("Player removed with connId " + conn.connectionId.ToString() + " and playerCntrlId " + player.playerControllerId.ToString());
    }

	//----------------------------------------------------------------------------------------

    public bool IsServerUp()
    {
        return NetworkServer.active;
    }

    #endregion

    #region Client

    public override void OnStartClient(NetworkClient client)
    {
        Debug.Log("Client started!");
		base.OnStartClient(client);
        client.Connect(serverIp, port);
    }

    public override void OnClientConnect(NetworkConnection conn)
    {
        Debug.Log("Client connected. ConnId = " + conn.connectionId.ToString());
        base.OnClientConnect(conn);
        _curConnectionId = conn.connectionId;        
    }

    public override void OnClientDisconnect(NetworkConnection conn)
    {
        Debug.Log("Client disconnected. ConnId = " + conn.connectionId.ToString());
        GameMng.LoadOfflineScene();
        base.OnClientDisconnect(conn);
        _curConnectionId = -1;
    }

	//----------------------------------------------------------------------------------------

	/// <summary>
	/// StopClient and StartClient correctly
	/// </summary>
    public void ResetClient()
    {
		StopClient();
		while(isNetworkActive)
		{
			//wait for the client to stop
		}
        StartClient();
    }

    #endregion

    #region Getters

    public int GetConnectionId()
    {
        return _curConnectionId;
    }

    public EMPLOYEE GetLoggedEmployee()
    {
        return _loggedEmployee;
    }

    #endregion

    #region Settters

    public void SetLoggegWorkerProfile(EMPLOYEE profile)
    {
        _loggedEmployee = profile;
    }

    #endregion

    #region IO
    
    private void ReadServerIPFromTextFile()
    {
        if (File.Exists(_documentsFolderPath + "serverIP.txt"))
        {
            using (FileStream fs = new FileStream(_documentsFolderPath + "serverIP.txt", FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                using (StreamReader sr = new StreamReader(fs))
                {
                    while (!sr.EndOfStream)
                    {
                         serverIp = sr.ReadLine().Trim();
                    }
                }
            }
        }
    }

    #endregion
}
