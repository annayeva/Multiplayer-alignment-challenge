using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.Networking;

public class RaycastManager : NetworkBehaviour
{
    //[SyncVar]
    //public Vector3 modelPositionState;
    //[SyncVar]
    //public Quaternion modelRotationState;

    private RaycastHit vision; // used for detecting raycast collision
    private float rayLength = 4.0f; // asigning length of the raycast

    private GameObject model; // the model to align
    private GameObject modelQR; // QR code that is a part of the model
    private GameObject roomQR; // QR code that is placed in the room

    private List<Cube> modelCubes = new List<Cube>(); //list of Cube objects storing cube meta data

    //Cube UI desplay elements
    private GameObject CubeIdText;
    private GameObject CubeNameText;
    private GameObject CubeInfoText;
    private GameObject CubePercentageText;
    private GameObject CubeEditText;
    private GameObject CubeDetailsText;
    private GameObject CubeUIPanel;

    void Start()
    {
        model = GameObject.Find("ModelToAlign(Clone)");

        CubeIdText = GameObject.Find("CubeId"); 
        CubeNameText = GameObject.Find("CubeName");
        CubeInfoText = GameObject.Find("CubeInfo");
        CubePercentageText = GameObject.Find("CubePercentage");
        CubeEditText = GameObject.Find("CubeLastEdit");
        CubeDetailsText = GameObject.Find("CubeDetailedDescription");
        CubeUIPanel = GameObject.Find("CubeUIPanel");
        CubeUIPanel.SetActive(false);
        //requesting cubes meta data
        StartCoroutine(GetRequest("https://nnedigitaldesignstorage.blob.core.windows.net/candidatetasks/Metadata.csv?sp=r&st=2021-03-15T09:12:39Z&se=2024-11-05T17:12:39Z&spr=https&sv=2020-02-10&sr=b&sig=oyj3Qyg4W42%2BO0d7YqmjxmKk0k%2BLVmE243ixdLaq3gk%3D"));
    }

    IEnumerator GetRequest(string uri)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(uri)) //fetching the data from URL
        {
            // request and wait for the desired page.
            yield return webRequest.SendWebRequest();

            string[] data;
            string[] rows = null;

            data = webRequest.downloadHandler.text.Split(new char[] { '\n' }); //separate data by new line

            for (int i = 1; i < data.Length - 1; i++)
            {
                rows = data[i].Split(new char[] { ';' }); //separate data by semicolon
                Cube cube = new Cube();
                cube.id = rows[0];
                cube.name = rows[1];
                cube.info = rows[2];
                cube.percentage = rows[3];
                cube.lastEdit = rows[4];
                cube.detailedDescription = rows[5];
                modelCubes.Add(cube);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out vision, rayLength)) //executes if a ray hits something
        {
            if (vision.collider.tag == "InteractiveQR") // checking the tag of the óbject that was hit by the ray
            {
                if (Input.GetMouseButtonDown(0)) // if while selecting an interactive object the mouse button was pressed call alignQrCodes()
                    alignQrCodes(vision.collider.gameObject); // passing the object that was hit
            }
            else if (vision.collider.tag == "CUBE") // checking the tag of the óbject that was hit by the ray
            {
                if (Input.GetMouseButtonDown(0)) // if while selecting an interactive object the mouse button was pressed call alignQrCodes()
                    handleCubeDisplay(vision.collider.gameObject); // passing the object that was hit
            }
        }
    }

    void handleCubeDisplay( GameObject clickedCube) {
        if(CubeUIPanel != null) 
            CubeUIPanel.SetActive(true);

        foreach (Cube cube in modelCubes) // browse in a list of Cube objects
        { 
            if (clickedCube.name.Equals(cube.name)) //search for the right cube
            {
                // Changing Text value in the UI
                CubeIdText.GetComponent<UnityEngine.UI.Text>().text = "ID: "+ cube.id;
                CubeNameText.GetComponent<UnityEngine.UI.Text>().text = "NAME: " + cube.name;
                CubeInfoText.GetComponent<UnityEngine.UI.Text>().text = "INFO:" + cube.info; 
                CubePercentageText.GetComponent<UnityEngine.UI.Text>().text = "PERCENTAGE: " + cube.percentage + "%";
                CubeEditText.GetComponent<UnityEngine.UI.Text>().text = "LAST EDIT: " + cube.lastEdit;
                CubeDetailsText.GetComponent<UnityEngine.UI.Text>().text = "DETAILED DESCRIPTION: " + cube.detailedDescription; 
            }
        }
    }

    void alignQrCodes(GameObject clickedQR)
    {
        roomQR = clickedQR;
        string QRCodeName;
        QRCodeName = roomQR.name;
        modelQR = model.transform.Find("QRcodes/" + QRCodeName).gameObject; // finding the modelQR by the same name as roomQR

        Quaternion deltaRotation = roomQR.transform.rotation * Quaternion.Inverse(modelQR.transform.rotation); // calculating difference in rotation for two matching QR codes
        model.transform.rotation = deltaRotation * model.transform.rotation; // applying delta rotation to the QR code parent object
        //Quaternion newAlignRotation = deltaRotation * model.transform.rotation;
        //CmdModelRotationState(newAlignRotation);
       
        Vector3 displacement = roomQR.transform.position - modelQR.transform.position; // calculating difference in position for two matching QR codes
        model.transform.position += displacement; // adding that displacement to the QR code parent object
        //sendAlignmentToServer(displacement);
        //Vector3 newAlignPosition = model.transform.position + displacement;
        //CmdModelPositionState(newAlignPosition);
    }

    //[Command(requiresAuthority = false)]
    //public void CmdModelRotationState(Quaternion newRotState, NetworkConnectionToClient sender = null)
    //{
    //        modelRotationState = newRotState;
    //        model.transform.rotation = modelRotationState;
    //}

    //[Command(requiresAuthority = false)]
    //public void CmdModelPositionState(Vector3 newPosState, NetworkConnectionToClient sender = null)
    //{
    //    modelPositionState = newPosState;
    //    model.transform.position = newPosState;
    //}

}
