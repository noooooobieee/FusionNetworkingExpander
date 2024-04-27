using LabFusion.Riptide.Utilities;
using LabFusion.Utilities;
using MelonLoader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BoneLib;
using TMPro;
using UnityEngine;
using Button = UnityEngine.UI.Button;
using System.Windows.Forms;

namespace RiptideNetworkLayer.BoneMenu
{
    [RegisterTypeInIl2Cpp]
    public class KeyboardCanvas : MonoBehaviour
    {
        public KeyboardCanvas(IntPtr intPtr) : base(intPtr) { }

        private bool isCaps = true;
        private bool isSpecial = true;
        private TMP_Text DisplayTMP;
        public Keyboard Keyboard;

        readonly List<Button> letters = [];
        readonly List<Button> specials = [];

        private void Awake()
        {
            UIMachineUtilities.OverrideFonts(transform);
            DontDestroyOnLoad(this);
        }

        public void SetupReferences()
        {
            DisplayTMP = transform.Find("Display").Find("Display Text").GetComponent<TMP_Text>();
            foreach (var button in transform.GetComponentsInChildren<Button>())
            {
                // Shut up I know this is dumb idc keyboards are dumb you are dumb die
                switch (button.gameObject.name)
                {
                    case "button_0":
                        button.AddClickEvent(() => OnClickKey("0"));
                        break;
                    case "button_1":
                        button.AddClickEvent(() => OnClickKey("1"));
                        break;
                    case "button_2":
                        button.AddClickEvent(() => OnClickKey("2"));
                        break;
                    case "button_3":
                        button.AddClickEvent(() => OnClickKey("3"));
                        break;
                    case "button_4":
                        button.AddClickEvent(() => OnClickKey("4"));
                        break;
                    case "button_5":
                        button.AddClickEvent(() => OnClickKey("5"));
                        break;
                    case "button_6":
                        button.AddClickEvent(() => OnClickKey("6"));
                        break;
                    case "button_7":
                        button.AddClickEvent(() => OnClickKey("7"));
                        break;
                    case "button_8":
                        button.AddClickEvent(() => OnClickKey("8"));
                        break;
                    case "button_9":
                        button.AddClickEvent(() => OnClickKey("9"));
                        break;
                    case "button_Q":
                        button.AddClickEvent(() => OnClickKey("Q"));
                        letters.Add(button);
                        break;
                    case "button_W":
                        button.AddClickEvent(() => OnClickKey("W"));
                        letters.Add(button);
                        break;
                    case "button_E":
                        button.AddClickEvent(() => OnClickKey("E"));
                        letters.Add(button);
                        break;
                    case "button_R":
                        button.AddClickEvent(() => OnClickKey("R"));
                        letters.Add(button);
                        break;
                    case "button_T":
                        button.AddClickEvent(() => OnClickKey("T"));
                        letters.Add(button);
                        break;
                    case "button_Y":
                        button.AddClickEvent(() => OnClickKey("Y"));
                        letters.Add(button);
                        break;
                    case "button_U":
                        button.AddClickEvent(() => OnClickKey("U"));
                        letters.Add(button);
                        break;
                    case "button_I":
                        button.AddClickEvent(() => OnClickKey("I"));
                        letters.Add(button);
                        break;
                    case "button_O":
                        button.AddClickEvent(() => OnClickKey("O"));
                        letters.Add(button);
                        break;
                    case "button_P":
                        button.AddClickEvent(() => OnClickKey("P"));
                        letters.Add(button);
                        break;
                    case "button_A":
                        button.AddClickEvent(() => OnClickKey("A"));
                        letters.Add(button);
                        break;
                    case "button_S":
                        button.AddClickEvent(() => OnClickKey("S"));
                        letters.Add(button);
                        break;
                    case "button_D":
                        button.AddClickEvent(() => OnClickKey("D"));
                        letters.Add(button);
                        break;
                    case "button_F":
                        button.AddClickEvent(() => OnClickKey("F"));
                        letters.Add(button);
                        break;
                    case "button_G":
                        button.AddClickEvent(() => OnClickKey("G"));
                        letters.Add(button);
                        break;
                    case "button_H":
                        button.AddClickEvent(() => OnClickKey("H"));
                        letters.Add(button);
                        break;
                    case "button_J":
                        button.AddClickEvent(() => OnClickKey("J"));
                        letters.Add(button);
                        break;
                    case "button_K":
                        button.AddClickEvent(() => OnClickKey("K"));
                        letters.Add(button);
                        break;
                    case "button_L":
                        button.AddClickEvent(() => OnClickKey("L"));
                        letters.Add(button);
                        break;
                    case "button_Z":
                        button.AddClickEvent(() => OnClickKey("Z"));
                        letters.Add(button);
                        break;
                    case "button_X":
                        button.AddClickEvent(() => OnClickKey("X"));
                        letters.Add(button);
                        break;
                    case "button_C":
                        button.AddClickEvent(() => OnClickKey("C"));
                        letters.Add(button);
                        break;
                    case "button_V":
                        button.AddClickEvent(() => OnClickKey("V"));
                        letters.Add(button);
                        break;
                    case "button_B":
                        button.AddClickEvent(() => OnClickKey("B"));
                        letters.Add(button);
                        break;
                    case "button_N":
                        button.AddClickEvent(() => OnClickKey("N"));
                        letters.Add(button);
                        break;
                    case "button_M":
                        button.AddClickEvent(() => OnClickKey("M"));
                        letters.Add(button);
                        break;
                    case "button_-":
                        button.AddClickEvent(() => OnClickKey("-"));
                        specials.Add(button);
                        break;
                    case "button__":
                        button.AddClickEvent(() => OnClickKey("_"));
                        specials.Add(button);
                        break;
                    case "button_=":
                        button.AddClickEvent(() => OnClickKey("="));
                        specials.Add(button);
                        break;
                    case "button_+":
                        button.AddClickEvent(() => OnClickKey("+"));
                        specials.Add(button);
                        break;
                    case "button_(":
                        button.AddClickEvent(() => OnClickKey("("));
                        specials.Add(button);
                        break;
                    case "button_)":
                        button.AddClickEvent(() => OnClickKey(")"));
                        specials.Add(button);
                        break;
                    case "button_[":
                        button.AddClickEvent(() => OnClickKey("["));
                        specials.Add(button);
                        break;
                    case "button_]":
                        button.AddClickEvent(() => OnClickKey("]"));
                        specials.Add(button);
                        break;
                    case "button_<":
                        button.AddClickEvent(() => OnClickKey("<"));
                        specials.Add(button);
                        break;
                    case "button_>":
                        button.AddClickEvent(() => OnClickKey(">"));
                        specials.Add(button);
                        break;
                    case "button_{":
                        button.AddClickEvent(() => OnClickKey("{"));
                        specials.Add(button);
                        break;
                    case "button_}":
                        button.AddClickEvent(() => OnClickKey("}"));
                        specials.Add(button);
                        break;
                    case "button_backSlash":
                        button.AddClickEvent(() => OnClickKey(@"\"));
                        specials.Add(button);
                        break;
                    case "button_forwardSlash":
                        button.AddClickEvent(() => OnClickKey("/"));
                        specials.Add(button);
                        break;
                    case "button_:":
                        button.AddClickEvent(() => OnClickKey(":"));
                        specials.Add(button);
                        break;
                    case "button_;":
                        button.AddClickEvent(() => OnClickKey(";"));
                        specials.Add(button);
                        break;
                    case "button_quote":
                        button.AddClickEvent(() => OnClickKey("\""));
                        specials.Add(button);
                        break;
                    case "button_'":
                        button.AddClickEvent(() => OnClickKey("'"));
                        specials.Add(button);
                        break;
                    case "button_`":
                        button.AddClickEvent(() => OnClickKey("`"));
                        specials.Add(button);
                        break;
                    case "button_.":
                        button.AddClickEvent(() => OnClickKey("."));
                        specials.Add(button);
                        break;
                    case "button_!":
                        button.AddClickEvent(() => OnClickKey("!"));
                        specials.Add(button);
                        break;
                    case "button_?":
                        button.AddClickEvent(() => OnClickKey("?"));
                        specials.Add(button);
                        break;
                    case "button_,":
                        button.AddClickEvent(() => OnClickKey(","));
                        specials.Add(button);
                        break;
                    case "button_#":
                        button.AddClickEvent(() => OnClickKey("#"));
                        specials.Add(button);
                        break;
                    case "button_$":
                        button.AddClickEvent(() => OnClickKey("$"));
                        specials.Add(button);
                        break;
                    case "button_&":
                        button.AddClickEvent(() => OnClickKey("&"));
                        specials.Add(button);
                        break;
                    case "button_Space":
                        button.AddClickEvent(() => OnClickKey(" "));
                        break;
                    case "button_Enter":
                        button.AddClickEvent(OnClickEnter);
                        break;
                    case "button_Caps":
                        button.AddClickEvent(OnCycleCaps);
                        break;
                    case "button_Cycle":
                        button.AddClickEvent(OnCycleSpecials);
                        break;
                    case "button_Cancel":
                        button.AddClickEvent(OnClickCancel);
                        break;
                    case "button_Back":
                        button.AddClickEvent(OnClickBack);
                        break;
                    case "button_Paste":
                        if (HelperMethods.IsAndroid())
                            button.gameObject.SetActive(false);
                        else
                            button.AddClickEvent(OnClickPaste);
                        break;
                }
            }

            OnCycleSpecials();
        }

        private void OnClickKey(string key)
        {
            if (isCaps)
                key = key.ToUpper();
            else
                key = key.ToLower();

            DisplayTMP.SetText(DisplayTMP.text += key);
        }

        private void OnClickPaste()
        {
            if (!Clipboard.ContainsText())
                return;

            var text = Clipboard.GetText();

            if (!string.IsNullOrWhiteSpace(text))
            {
                DisplayTMP.SetText(DisplayTMP.text += text);
            }
        }

        private void OnClickBack()
        {
            DisplayTMP.SetText(DisplayTMP.text.Remove(DisplayTMP.text.Length - 1, 1));
        }
        
        private void OnClickCancel()
        {
            DisplayTMP.SetText("");

            BoneLib.BoneMenu.MenuManager.SelectCategory(Keyboard.Category.Parent);
        }

        private void OnClickEnter()
        {
            BoneLib.BoneMenu.MenuManager.SelectCategory(Keyboard.Category.Parent);

            Keyboard.OnEnter?.Invoke(DisplayTMP.text);

            DisplayTMP.SetText("");
        }

        private void OnCycleCaps()
        {
            isCaps = !isCaps;
            foreach (var key in letters)
            {
                if (isCaps)
                    key.transform.GetComponentInChildren<TMP_Text>().SetText(key.transform.GetComponentInChildren<TMP_Text>().text.ToUpper());
                else
                    key.transform.GetComponentInChildren<TMP_Text>().SetText(key.transform.GetComponentInChildren<TMP_Text>().text.ToLower());
            }
        }

        private void OnCycleSpecials()
        {
            isSpecial = !isSpecial;

            foreach (var key in letters)
            {
                if (!isSpecial)
                    key.gameObject.SetActive(true);
                else
                    key.gameObject.SetActive(false);
            }

            foreach (var key in specials)
            {
                if (isSpecial)
                    key.gameObject.SetActive(true);
                else
                    key.gameObject.SetActive(false);
            }
        }
    }
}
