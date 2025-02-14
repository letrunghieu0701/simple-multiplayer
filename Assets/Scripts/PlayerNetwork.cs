using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public class PlayerNetwork : NetworkBehaviour
{
    [SerializeField] private float moveSpeed = 3f;

    private Rigidbody rigid;
    [SerializeField] private float jumpForce = 7f;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float raycastDistance = 0.6f;
    private bool isGrounded = false;

    private TMP_Text playerIdText;

    [SerializeField] private Transform spawnPrefab;
    private Transform spawnedTrs;

    private NetworkVariable<int> netVar = new NetworkVariable<int>(5);
    private NetworkVariable<CustomData> netCustomData = new NetworkVariable<CustomData>();

    public struct CustomData : INetworkSerializable {
        public int _int;
        public bool _bool;
        public FixedString128Bytes _fixedString;

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref _int);
            serializer.SerializeValue(ref _bool);
            serializer.SerializeValue(ref _fixedString);
        }
    }

    override public void OnNetworkSpawn()
    {
        this.transform.position = new Vector3(0, 3f, 0);

        playerIdText.text = $"Player ID: {this.NetworkObjectId}";
        playerIdText.transform.LookAt(Camera.main.transform);
        playerIdText.transform.Rotate(new Vector3(0, 180, 0));

        netVar.OnValueChanged += (int previousValue, int newValue) =>
        {
            spawnedTrs = Instantiate(spawnPrefab);
            spawnedTrs.GetComponent<NetworkObject>().Spawn(true);

            //TestServerRpc();
            //TestClientRpc();

            //Debug.Log($"New Value: {newValue}");

            //netCustomData.Value = new CustomData
            //{
            //    _int = 4,
            //    _bool = true,
            //    _string = "haha",
            //    _fixedString = "hehe",
            //};
            //Debug.Log($"_int: {netCustomData.Value._int} _bool: {netCustomData.Value._bool}" +
            //    $" _string: {netCustomData.Value._string} _fixedString: {netCustomData.Value._fixedString}");
        };
    }

    private void Awake()
    {
        rigid = this.transform.GetComponent<Rigidbody>();
        Transform trs = this.transform.Find("Canvas/PlayerIdText");
        playerIdText = this.transform.Find("Canvas/PlayerIdText").GetComponent<TMP_Text>();
    }

    private void Update()
    {
        // If not owner of this object, then do nothing
        if (!this.IsOwner)
        {
            return;
        }



        // Movements
        Vector3 moveDirection = new Vector3(0, 0, 0);
        if (Input.GetKey(KeyCode.W)) moveDirection = Vector3.forward;
        if (Input.GetKey(KeyCode.S)) moveDirection = Vector3.back;
        if (Input.GetKey(KeyCode.A)) moveDirection = Vector3.left;
        if (Input.GetKey(KeyCode.D)) moveDirection = Vector3.right;
        this.transform.position += moveDirection * moveSpeed * Time.deltaTime;

        Vector3 endOfRay = new Vector3(this.transform.position.x, this.transform.position.y - raycastDistance, this.transform.position.z);
        Debug.DrawLine(this.transform.position, endOfRay, Color.red);

        // Ground check
        RaycastHit hit;
        if (Physics.Raycast(this.transform.position, Vector3.down, out hit, raycastDistance, groundLayer))
        {
            isGrounded = true;
        }
        else
        {
           isGrounded = false;
        }
        Debug.Log($"isGrounded: {isGrounded}");

        // Jump
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            //netVar.Value = Random.Range(1, 100);
            rigid.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }

        //if (Input.GetKeyDown(KeyCode.Y))
        //{
        //    spawnedTrs.GetComponent<NetworkObject>().Despawn(true);
        //    //Destroy(spawnedTrs.gameObject);
        //}

        //if (Input.GetKeyDown(KeyCode.B))
        //{
        //    Debug.Log($"Owner ID: {this.NetworkObjectId}");
        //}

        //if (Input.GetKeyDown(KeyCode.X))
        //{
        //    TestServerRpc();
        //}

        //if (Input.GetKeyDown(KeyCode.C))
        //{
        //    TestClientRpc();
        //}
    }
    
    // Call in Client, Run in Server
    [ServerRpc]
    private void TestServerRpc()
    {
        Debug.Log($"Owner ID: {this.NetworkObjectId}");
    }

    // Call in Server, Run in Client
    [ClientRpc]
    private void TestClientRpc()
    {
        Debug.Log($"Owner ID: {this.NetworkObjectId}");
    }
}
