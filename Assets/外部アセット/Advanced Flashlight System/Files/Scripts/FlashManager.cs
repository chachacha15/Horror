using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEditor;

/*
 * if an error appear here... You have to install Unity New Input System Package.
 * Go to:
 * Window -> Package Manager -> Select Packages: Unity Registry -> Search for (Input System) -> Press Install.
 * The Error should disappear now.
 */
using UnityEngine.InputSystem;

namespace M4DOOM
{
    public class FlashManager : MonoBehaviour
    {


        #region Variables

        public int MyInt = 5;
        //Track Camera.
        public Camera TrackCamera;
        //Track Camera.
        public Transform CameraIntractPointer;
        //Flash Light.
        public Light LightSource;

        //Light Texture Cookie.
        public Texture LightCookie;

        //Battery Life Span.
        public int BatteryLife;
        //Battery Count.
        public int BatteryCount;
        //Intract Range.
        public int BatteryPickRange;

        //Track Speed.
        public float TrackSpeed;

        //Using Noise.
        public bool UseIntensity;
        //Using Noise.
        public bool UseNoise;
        //Using Motion.
        public bool UseMotion;
        //Using Battery Life.
        public bool UseBatteryLife;
        //Using New Input System.
        public bool UseNewInputSystem;

        //Intract Keys
        public KeyCode UseKey;
        public KeyCode PickBatteryKey;
        public KeyCode ReloadKey;

        public InputActionReference Use_Action;
        public InputActionReference PickBattery_Action;
        public InputActionReference ReloadKey_Action;


        //Sound Effects
        public AudioSource SwitchOnSFX;
        public AudioSource SwitchOffSFX;
        public AudioSource NoiseSFX;

        //Instruction Display.
        public Text inGameText;
        //Battery Count Display.
        public Text BatteryCountText;
        //Battery Life Display.
        public Slider BatterySlider;
        //Screen Middle Point Display.
        public Image NormalImage;
        //Screen Hand Display.
        public Image HandImage;

        //Battery Lifespan.
        float DrainTime;
        //Rotate Time.
        float RotationTime = 2;

        //Flash Status.
        bool LightOn;
        //One Method Call.
        bool OneCall;
        //On Noise.
        bool NoiseLight;

        //Look Ray. 
        Ray ray;

        //Light Rotation.
        Quaternion LightRotation;

        //Vector2 Motion Values.
        Vector2[] XYMotion = new Vector2[2];
        Vector2 Xvalue;
        Vector2 Yvalue;

        //Vector3 Motion Values.
        Vector3 RotateValues = new Vector3(1, 1, 0);
        Vector3 XYvalue;

        #endregion

        #region Methods
        //At Start.
        private void Start()
        {
            //Check if the Light is Infinite...Disable FlashLight UI
            if (!UseBatteryLife)
            {
                BatteryCountText.transform.parent.gameObject.SetActive(false);
            }

            //if flash is using noise.
            if (UseNoise)
            {
                //Call NoiseCaller Mehtod.
                StartCoroutine(NoiseCaller());
            }

            //Set Battery Life span Max Value to BatteryLife.
            BatterySlider.maxValue = BatteryLife;

            //Set LightCookie Texture to Flashlight.
            LightSource.cookie = LightCookie;

            //Call UseBattery Method.
            UseBattery();

            //Assing light rotation.
            LightRotation = LightSource.transform.localRotation;

            //Set Intial values to rotate vectors.
            for (int i = 0; i < 2; i++)
            {
                XYMotion[i].Set(Mathf.Cos(Random.value), Mathf.Sin(Random.value));
            }
        }

        //Every Frame.
        private void Update()
        {
            //Check the Input.
            if (UseNewInputSystem)
            {
                //Input Action is Use_Action.
                if (Use_Action.action.triggered)
                {
                    //Call FlashModeChanger Mehtod.
                    FlashModeChanger();
                }
                //Input Action is ReloadKey_Action.
                else if (ReloadKey_Action.action.triggered)
                {
                    //Call UseBattery Mehtod.
                    UseBattery();
                }
            }
            else
            {
                //Input Key is UseKey.
                if (Input.GetKeyDown(UseKey))
                {
                    //Call FlashModeChanger Mehtod.
                    FlashModeChanger();
                }
                //Input Key is ReloadKey.
                else if (Input.GetKeyDown(ReloadKey))
                {
                    //Call UseBattery Mehtod.
                    UseBattery();
                }
            }

            //Create Ray from Camera to forward direction.
            ray = new Ray(CameraIntractPointer.position, CameraIntractPointer.forward);

            
            //if Ray hit Battery Layer.
            if (Physics.Raycast(ray, out RaycastHit hit, BatteryPickRange) && hit.transform.gameObject.name.StartsWith("Battery"))
            {

                //Enable Hand Image.
                HandImage.enabled = true;
                //Disable Hand Image.
                NormalImage.enabled = false;

                //Display Text.
                inGameText.text = "[E] : �E��";

                if (UseNewInputSystem)
                {

                    //Input Action is PickBattery_Action.
                    if (PickBattery_Action.action.triggered)
                    {
                        //Call addBatery Method.
                        AddBattery();

                        //Destroy Battery Object.
                        Destroy(hit.transform.gameObject);
                    }
                }
                else
                {

                    //Input Key is PickBatteryKey.
                    if (Input.GetKeyDown(PickBatteryKey) || Input.GetMouseButtonDown(0))
                    {
                        //Call addBatery Method.
                        AddBattery();

                        //Destroy Battery Object.
                        Destroy(hit.transform.gameObject);
                    }
                }
            }
            else
            {
                //Disable Hand Image.
                HandImage.enabled = false;
                //Enable Hand Image.
                NormalImage.enabled = true;

                //Display Empty Text.
                inGameText.text = string.Empty;
            }

            //if Battery Life is used it will drain the battery life.
            if (UseBatteryLife)
            {
                //Light is On and DrainTime more than or equal 0.
                if (LightOn && DrainTime >= 0)
                {
                    //Decrement DrainTime.
                    DrainTime -= Time.deltaTime * 0.2f;

                    //drainTimeF to DrainTime / BatteryLife.
                    float drainTimeF = DrainTime / BatteryLife;

                    //If Use Intensity System.
                    if (UseIntensity)
                    {
                        //Flash Intensity to drainTimeF.
                        LightSource.intensity = drainTimeF;
                    }
                    //Battery Slider to drainTimeF.
                    BatterySlider.value = DrainTime;

                    //DrainTime less than or equal 0 and OneCall is true.
                    if (DrainTime <= 0 && OneCall)
                    {
                        //Call FlashModeChanger Mehtod.
                        FlashModeChanger();

                        //Set OneCall to false.
                        OneCall = false;
                    }
                }
            }

            //if is using Motion, it will move the light like real life hand.
            if (UseMotion)
            {
                //Increase RotationTime.
                RotationTime += Time.deltaTime * 0.2f;

                //Calculate rotate values.
                Xvalue = XYMotion[0] * RotationTime;
                Yvalue = XYMotion[1] * RotationTime;
                XYvalue = new Vector3((Mathf.PerlinNoise(Xvalue.x, Xvalue.y) - 0.5f), (Mathf.PerlinNoise(Yvalue.x, Yvalue.y) - 0.5f), 0);

                //Set new Rotation tp LightSource.
                LightSource.transform.localRotation = Quaternion.Euler(Vector3.Scale(XYvalue, RotateValues) * 4) * LightRotation;
            }

            //Set position to be behind TrackCamera.
            transform.position = TrackCamera.transform.position + -(TrackCamera.transform.forward);
            //Rotation to Slerp(Camera Rotation * Track Speed * FlashModeChanger).
            transform.rotation = Quaternion.Slerp(transform.rotation, TrackCamera.transform.rotation, TrackSpeed * Time.deltaTime);
        }

        //To Repeat Noise On Flash.
        private IEnumerator NoiseCaller()
        {
            //Wait for Random seconds in (25 - 50).
            yield return new WaitForSeconds(Random.Range(25, 50));

            //If flash is on.
            if (LightOn)
            {
                //Set NoiseLight to true.
                NoiseLight = true;

                //Call Noise Mehtod.
                StartCoroutine(Noise());

                //Call NoiseTimer Mehtod.
                StartCoroutine(NoiseTimer());
            }

            //Call NoiseCaller Mehtod.
            StartCoroutine(NoiseCaller());
        }

        //To apply noise on flash.
        private IEnumerator Noise()
        {
            //Wait for Random seconds in (0.01f, 0.15f).
            yield return new WaitForSeconds(Random.Range(0.01f, 0.15f));

            //If NoiseLight is true.
            if (NoiseLight)
            {
                //Call FlashModeChanger Method.
                FlashModeChanger();

                //Call Noise Mehtod.
                StartCoroutine(Noise());
            }
            else
            {
                //If flash is off.
                if (!LightOn)
                {
                    //Enable Light Source.
                    LightSource.enabled = true;

                    //LightOn to true.
                    LightOn = true;
                }
            }
        }

        //Timer for Noise.
        private IEnumerator NoiseTimer()
        {
            //Play Noise Sound Effect.
            NoiseSFX.Play();

            //Wait for Random seconds in (1f, 2).
            yield return new WaitForSeconds(Random.Range(1f, 2f));

            //Pause Noise Sound Effect.
            NoiseSFX.Pause();

            //Set NoiseLight to false.
            NoiseLight = false;
        }

        //Method to Change Flash Mode.
        private void FlashModeChanger()
        {
            //If flash is on.
            if (LightOn)
            {
                //Disable Light Source.
                LightSource.enabled = false;

                //LightOn to false.
                LightOn = false;

                //If NoiseLight is false.
                if (!NoiseLight)
                {
                    //Play Switch Off Sound Effect.
                    SwitchOffSFX.Play();
                }
            }
            else
            {
                //Enable Light Source.
                LightSource.enabled = true;

                //LightOn to true.
                LightOn = true;

                //If NoiseLight is false.
                if (!NoiseLight)
                {
                    //Play Switch On Sound Effect.
                    SwitchOnSFX.Play();
                }
            }
        }

        //Method to Use Battery.
        private void UseBattery()
        {
            //If batteries more than 0 and DrainTime less than BatteryLife/3.
            if (BatteryCount > 0 && DrainTime < BatteryLife / 2)
            {
                //DrainTime to Battery Life span.
                DrainTime = BatteryLife;

                //BatterySlider to BatteryLife.
                BatterySlider.value = BatteryLife;

                //LightSource intensity to 1.
                LightSource.intensity = 300;
                
                //Decrement Batteries.
                BatteryCount--;

                //Call DisplayBattery Method.
                DisplayBattery();

                //Set NoiseLight to true.
                NoiseLight = true;

                //Call Noise Method.
                StartCoroutine(Noise());

                //Call NoiseTimer Method.
                StartCoroutine(NoiseTimer());


                //OneCall to true.
                OneCall = true;
            }
        }

        //Method to Increment Battries.
        private void AddBattery()
        {
            //Increment Battries.
            BatteryCount++;

            //Call DisplayBattery Method.
            DisplayBattery();
        }

        //Method to display Battries.
        private void DisplayBattery()
        {
            //Display Batteries.
            BatteryCountText.text = "" + BatteryCount;
        }

        #endregion
    }

    #region Editor GUI

#if UNITY_EDITOR
    [CustomEditor(typeof(FlashManager)), InitializeOnLoadAttribute]
    public class FlashManagerEditor : Editor
    {
        FlashManager fm;
        SerializedObject SerFM;

        private void OnEnable()
        {
            fm = (FlashManager)target;
            SerFM = new SerializedObject(fm);
        }

        public override void OnInspectorGUI()
        {
            SerFM.Update();

            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
            GUILayout.Label("Advanced FlashLight System\nBy M4DOOM", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter, fontStyle = FontStyle.Bold, fontSize = 18 }, GUILayout.ExpandWidth(true));
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
            EditorGUILayout.Space();

            GUILayout.Label("Flash", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleLeft, fontStyle = FontStyle.Bold, fontSize = 13 }, GUILayout.ExpandWidth(true));
            fm.LightSource = (Light)EditorGUILayout.ObjectField(new GUIContent("Light Source", "FlashLigt Lightning source."), fm.LightSource, typeof(Light), true);
            fm.LightCookie = (Texture)EditorGUILayout.ObjectField(new GUIContent("Light Cookie", "Light Display Texture."), fm.LightCookie, typeof(Texture), true);
            fm.UseIntensity = EditorGUILayout.ToggleLeft(new GUIContent("Use Intensity", "if true, flash will be at full power until battery died."), fm.UseIntensity);
            fm.UseMotion = EditorGUILayout.ToggleLeft(new GUIContent("Use Motion", "if true, flash will rotate realistically."), fm.UseMotion);
            fm.UseNoise = EditorGUILayout.ToggleLeft(new GUIContent("Use Noise", "if true, flash will go on and off rapidly."), fm.UseNoise);
            fm.UseBatteryLife = EditorGUILayout.ToggleLeft(new GUIContent("Use Battery Life", "if true, flash will use battery life to stay On."), fm.UseBatteryLife);
            fm.UseNewInputSystem = EditorGUILayout.ToggleLeft(new GUIContent("Use New Input System", "if true, the system will only work with Unity Input System Package."), fm.UseNewInputSystem);
            EditorGUILayout.Space();

            GUILayout.Label("Flash Tracking", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleLeft, fontStyle = FontStyle.Bold, fontSize = 13 }, GUILayout.ExpandWidth(true));
            fm.TrackCamera = (Camera)EditorGUILayout.ObjectField(new GUIContent("Track Camera", "Flash will follow camera movement."), fm.TrackCamera, typeof(Camera), true);
            fm.TrackSpeed = EditorGUILayout.Slider(new GUIContent("Track Speed", "Flash tracking speed."), fm.TrackSpeed, 5, 10);
            fm.CameraIntractPointer = (Transform)EditorGUILayout.ObjectField(new GUIContent("Camera Intract Pointer", "Camera Intract Pointer."), fm.CameraIntractPointer, typeof(Transform), true);
            EditorGUILayout.Space();

            GUILayout.Label("Battery", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleLeft, fontStyle = FontStyle.Bold, fontSize = 13 }, GUILayout.ExpandWidth(true));
            fm.BatteryLife = EditorGUILayout.IntSlider(new GUIContent("Battery Life", "Battery life span."), fm.BatteryLife, 20, 300);
            fm.BatteryCount = EditorGUILayout.IntSlider(new GUIContent("Battery Count", "Start battery count."), fm.BatteryCount, 0, 30);
            fm.BatteryPickRange = EditorGUILayout.IntSlider(new GUIContent("Battery Pick Range", "Distace to intract with battery."), fm.BatteryPickRange, 0, 220);
            EditorGUILayout.Space();

            GUILayout.Label("User Interface", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleLeft, fontStyle = FontStyle.Bold, fontSize = 13 }, GUILayout.ExpandWidth(true));
            fm.inGameText = (Text)EditorGUILayout.ObjectField(new GUIContent("Game Text", "Game instruction text display."), fm.inGameText, typeof(Text), true);
            fm.BatteryCountText = (Text)EditorGUILayout.ObjectField(new GUIContent("Battery Count Text", "Battery count text display."), fm.BatteryCountText, typeof(Text), true);
            fm.BatterySlider = (Slider)EditorGUILayout.ObjectField(new GUIContent("Battery Drain Slider", "Battery life span slider display."), fm.BatterySlider, typeof(Slider), true);
            fm.NormalImage = (Image)EditorGUILayout.ObjectField(new GUIContent("Screen Pointer", "screen pointer image."), fm.NormalImage, typeof(Image), true);
            fm.HandImage = (Image)EditorGUILayout.ObjectField(new GUIContent("Screen Hand", "screen hand pointer image."), fm.HandImage, typeof(Image), true);
            EditorGUILayout.Space();

            GUILayout.Label("Intracting Keys", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleLeft, fontStyle = FontStyle.Bold, fontSize = 13 }, GUILayout.ExpandWidth(true));
            GUILayout.Label("Old Input System", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleLeft, fontStyle = FontStyle.Bold, fontSize = 13 }, GUILayout.ExpandWidth(true));
            fm.UseKey = (KeyCode)EditorGUILayout.EnumPopup(new GUIContent("Use Flash Key", "Toggle flash light."), fm.UseKey);
            fm.ReloadKey = (KeyCode)EditorGUILayout.EnumPopup(new GUIContent("Reload Key", "Reload flash battery."), fm.ReloadKey);
            fm.PickBatteryKey = (KeyCode)EditorGUILayout.EnumPopup(new GUIContent("Pick Battery Key", "Pick up battery."), fm.PickBatteryKey);

            //New Input System.
            GUILayout.Label("New Input System", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleLeft, fontStyle = FontStyle.Bold, fontSize = 13 }, GUILayout.ExpandWidth(true));
            fm.Use_Action = (InputActionReference)EditorGUILayout.ObjectField(new GUIContent("Use Flash Action", "Toggle flash light."), fm.Use_Action, typeof(InputActionReference),false);
            fm.ReloadKey_Action = (InputActionReference)EditorGUILayout.ObjectField(new GUIContent("Reload Action", "Reload flash battery."), fm.ReloadKey_Action, typeof(InputActionReference),false);
            fm.PickBattery_Action = (InputActionReference)EditorGUILayout.ObjectField(new GUIContent("Pick Battery Action", "Pick up battery."), fm.PickBattery_Action, typeof(InputActionReference),false);
            
            
            EditorGUILayout.Space();

            GUILayout.Label("Sound Effects", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleLeft, fontStyle = FontStyle.Bold, fontSize = 13 }, GUILayout.ExpandWidth(true));
            fm.SwitchOnSFX = (AudioSource)EditorGUILayout.ObjectField(new GUIContent("Switch On", "Play when turn flash on."), fm.SwitchOnSFX, typeof(AudioSource), true);
            fm.SwitchOffSFX = (AudioSource)EditorGUILayout.ObjectField(new GUIContent("Switch Off", "Play when turn flash off."), fm.SwitchOffSFX, typeof(AudioSource), true);
            fm.NoiseSFX = (AudioSource)EditorGUILayout.ObjectField(new GUIContent("Noise", "Play when flash enter noise."), fm.NoiseSFX, typeof(AudioSource), true);

            if (GUI.changed)
            {
                EditorUtility.SetDirty(fm);
                Undo.RecordObject(fm, "FM Change");
                SerFM.ApplyModifiedProperties();
            }
        }
    }
#endif

    #endregion
}