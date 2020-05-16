using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KModkit;

public class Reaction : MonoBehaviour {

    public KMAudio Audio;
    public KMBombInfo Bomb;

    static int moduleIdCounter = 1;
    int moduleId;

    public KMSelectable[] buttons;
    public GameObject[] cableSetupIndicators;
    public MeshRenderer[] leds;
    public Material[] ledColors;

    private int button0;
    private int button1;
    private int button2;
    private int button3;
    private int button4;
    private int button5;

    private float timer = 0;
    private int onNumber;

    private int cableSetupNumber = 0;
    private bool active = false;

    private int buttonNumber;

    void Awake()
    {
        moduleId = moduleIdCounter++;
        foreach (KMSelectable button in buttons)
        {
            KMSelectable pressedButton = button;
            button.OnInteract += delegate () { ButtonPress(pressedButton); return false; };
        }
        GetComponent<KMNeedyModule>().OnNeedyActivation += OnNeedyActivation;
        GetComponent<KMNeedyModule>().OnNeedyDeactivation += OnNeedyDeactivation;
    }

    private void Start()
    {
        foreach (MeshRenderer led in leds)
        {
            led.material = ledColors[1];
        }
        foreach (GameObject cableSteupIndicator in cableSetupIndicators)
        {
            cableSteupIndicator.GetComponent<MeshRenderer>().material = ledColors[3];
        }
    }

    protected void OnNeedyActivation()
    {
        foreach (GameObject cableSteupIndicator in cableSetupIndicators)
        {
            cableSteupIndicator.GetComponent<MeshRenderer>().material = ledColors[2];
            if (UnityEngine.Random.Range(0, 2) == 1)
            {
                cableSteupIndicator.SetActive(false);
            } else {
                cableSetupNumber += 1;
            }
        }
        Debug.Log("[Reaction #" + moduleId + "] There are " + cableSetupNumber + " white markings.");
        if (cableSetupNumber == 0)
        {
            button0 = 4;
            button1 = 5;
            button2 = 1;
            button3 = 2;
            button4 = 0;
            button5 = 3;
        }
        if (cableSetupNumber == 1)
        {
            button0 = 2;
            button1 = 5;
            button2 = 1;
            button3 = 4;
            button4 = 0;
            button5 = 3;
        }
        if (cableSetupNumber == 2)
        {
            button0 = 3;
            button1 = 0;
            button2 = 1;
            button3 = 4;
            button4 = 5;
            button5 = 2;
        }
        if (cableSetupNumber == 3)
        {
            button0 = 0;
            button1 = 4;
            button2 = 2;
            button3 = 3;
            button4 = 1;
            button5 = 5;
        }
        if (cableSetupNumber == 4)
        {
            button0 = 0;
            button1 = 2;
            button2 = 3;
            button3 = 1;
            button4 = 5;
            button5 = 4;
        }
        if (cableSetupNumber == 5)
        {
            button0 = 5;
            button1 = 0;
            button2 = 3;
            button3 = 4;
            button4 = 1;
            button5 = 2;
        }
        if (cableSetupNumber == 6)
        {
            button0 = 5;
            button1 = 3;
            button2 = 2;
            button3 = 0;
            button4 = 4;
            button5 = 1;
        }
        if (cableSetupNumber == 7)
        {
            button0 = 1;
            button1 = 4;
            button2 = 0;
            button3 = 5;
            button4 = 2;
            button5 = 3;
        }
        if (cableSetupNumber == 8)
        {
            button0 = 4;
            button1 = 1;
            button2 = 2;
            button3 = 3;
            button4 = 5;
            button5 = 0;
        }
        Debug.Log("[Reaction #" + moduleId + "] The buttons, from left to right, are connected to the following lights: " + (button0 + 1) + "th, " + (button1 + 1) + "th, " + (button2 + 1) + "th, " + (button3 + 1) + "th, " + (button4 + 1) + "th, and " + (button5 + 1) + "th.");
        active = true;
    }

    private void Update()
    {
        if (active)
        {
            onNumber = 0;
            foreach (MeshRenderer led in leds)
            {
                if (led.sharedMaterial == ledColors[0])
                {
                    onNumber += 1;
                }
            }
            if (onNumber == 6)
            {
                Debug.Log("[Reaction #" + moduleId + "] All LEDs are on at the same time. STRIKE!");
                GetComponent<KMNeedyModule>().HandleStrike();
                foreach (MeshRenderer led in leds)
                {
                    led.sharedMaterial = ledColors[1];
                }
            }
            timer += Time.deltaTime;
            if (timer >= 1)
            {
                LightLoop();
                timer -= 1;
            }
        }
    }

    void LightLoop()
    {
        if (active)
        {
            if (onNumber == 5)
            {
                Audio.PlaySoundAtTransform("ReactionAlarm", transform);
                Debug.Log("[Reaction #" + moduleId + "] Warning! 5/6 LEDs are on!");
            }
            if (UnityEngine.Random.Range(0, (((Bomb.GetModuleNames().Count) - (Bomb.GetSolvableModuleNames().Count)) + ((Bomb.GetSolvableModuleNames().Count) - (Bomb.GetSolvedModuleNames().Count)))) == 0)
            {
                leds[UnityEngine.Random.Range(0, leds.Length)].material = ledColors[0];
            }
        }
    }

    protected void OnNeedyDeactivation()
    {
        active = false;
    }

    private void ButtonPress(KMSelectable pressedButton)
    {
        if (active)
        {
            Debug.Log("[Reaction #" + moduleId + "] Player pressed " + pressedButton.gameObject.name + ". (Counting from left to right, starting with 0), and succesfully turned off an LED.");
            pressedButton.AddInteractionPunch(1f);
            GetComponent<KMAudio>().PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.BigButtonRelease, pressedButton.transform);
            GetComponent<KMAudio>().PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.WireSequenceMechanism, transform);
            buttonNumber = int.Parse(pressedButton.gameObject.name.ToArray().Last().ToString().ToString());
            if (buttonNumber == 0)
            {
                if (leds[button0].sharedMaterial == ledColors[0])
                {
                    leds[button0].sharedMaterial = ledColors[1];
                } else {
                    GetComponent<KMAudio>().PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.BigButtonRelease, pressedButton.transform);
                    pressedButton.AddInteractionPunch(2f);
                    Debug.Log("[Reaction #" + moduleId + "] Player pressed " + pressedButton.gameObject.name + " (Counting from left to right, starting with 0), and that button was wired to LED #" + button0 + " (Counting from left to right, starting with 0), and that light was off. STRIKE!");
                    GetComponent<KMNeedyModule>().HandleStrike();
                    foreach (MeshRenderer led in leds)
                    {
                        led.sharedMaterial = ledColors[1];
                    }
                }
            }
            if (buttonNumber == 1)
            {
                if (leds[button1].sharedMaterial == ledColors[0])
                {
                    leds[button1].sharedMaterial = ledColors[1];
                }
                else
                {
                    GetComponent<KMAudio>().PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.BigButtonRelease, pressedButton.transform);
                    pressedButton.AddInteractionPunch(2f);
                    Debug.Log("[Reaction #" + moduleId + "] Player pressed " + pressedButton.gameObject.name + " (Counting from left to right, starting with 0), and that button was wired to LED #" + button0 + " (Counting from left to right, starting with 0), and that light was off. STRIKE!");
                    GetComponent<KMNeedyModule>().HandleStrike();
                    foreach (MeshRenderer led in leds)
                    {
                        led.sharedMaterial = ledColors[1];
                    }
                }
            }
            if (buttonNumber == 2)
            {
                if (leds[button2].sharedMaterial == ledColors[0])
                {
                    leds[button2].sharedMaterial = ledColors[1];
                }
                else
                {
                    GetComponent<KMAudio>().PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.BigButtonRelease, pressedButton.transform);
                    pressedButton.AddInteractionPunch(2f);
                    Debug.Log("[Reaction #" + moduleId + "] Player pressed " + pressedButton.gameObject.name + " (Counting from left to right, starting with 0), and that button was wired to LED #" + button0 + " (Counting from left to right, starting with 0), and that light was off. STRIKE!");
                    GetComponent<KMNeedyModule>().HandleStrike();
                    foreach (MeshRenderer led in leds)
                    {
                        led.sharedMaterial = ledColors[1];
                    }
                }
            }
            if (buttonNumber == 3)
            {
                if (leds[button3].sharedMaterial == ledColors[0])
                {
                    leds[button3].sharedMaterial = ledColors[1];
                }
                else
                {
                    GetComponent<KMAudio>().PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.BigButtonRelease, pressedButton.transform);
                    pressedButton.AddInteractionPunch(2f);
                    Debug.Log("[Reaction #" + moduleId + "] Player pressed " + pressedButton.gameObject.name + " (Counting from left to right, starting with 0), and that button was wired to LED #" + button0 + " (Counting from left to right, starting with 0), and that light was off. STRIKE!");
                    GetComponent<KMNeedyModule>().HandleStrike();
                    foreach (MeshRenderer led in leds)
                    {
                        led.sharedMaterial = ledColors[1];
                    }
                }
            }
            if (buttonNumber == 4)
            {
                if (leds[button4].sharedMaterial == ledColors[0])
                {
                    leds[button4].sharedMaterial = ledColors[1];
                }
                else
                {
                    GetComponent<KMAudio>().PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.BigButtonRelease, pressedButton.transform);
                    pressedButton.AddInteractionPunch(2f);
                    Debug.Log("[Reaction #" + moduleId + "] Player pressed " + pressedButton.gameObject.name + " (Counting from left to right, starting with 0), and that button was wired to LED #" + button0 + " (Counting from left to right, starting with 0), and that light was off. STRIKE!");
                    GetComponent<KMNeedyModule>().HandleStrike();
                    foreach (MeshRenderer led in leds)
                    {
                        led.sharedMaterial = ledColors[1];
                    }
                }
            }
            if (buttonNumber == 5)
            {
                if (leds[button5].sharedMaterial == ledColors[0])
                {
                    leds[button5].sharedMaterial = ledColors[1];
                }
                else
                {
                    GetComponent<KMAudio>().PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.BigButtonRelease, pressedButton.transform);
                    pressedButton.AddInteractionPunch(2f);
                    Debug.Log("[Reaction #" + moduleId + "] Player pressed " + pressedButton.gameObject.name + " (Counting from left to right, starting with 0), and that button was wired to LED #" + (button0 + 1) + ", and that light was off. STRIKE!");
                    GetComponent<KMNeedyModule>().HandleStrike();
                    foreach (MeshRenderer led in leds)
                    {
                        led.sharedMaterial = ledColors[1];
                    }
                }
            }
        } else {
            GetComponent<KMAudio>().PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonRelease, pressedButton.transform);
            pressedButton.AddInteractionPunch(0.5f);
        }
    }
}
