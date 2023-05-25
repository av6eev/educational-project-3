using Descriptions.Base;
using Plugins.DiscordUnity;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Start
{
    public class StartView : MonoBehaviour
    {
        public DescriptionsCollectionSo DescriptionsCollection;
        public DiscordManager DiscordManager;
        
        public GameObject LoaderCanvas;
        public GameObject InvalidKeyCanvas;
        
        public Image ProgressBar;
        public TextMeshProUGUI LoadingTxt;
        public TMP_InputField KeyInputField;
        public Button NewGameBtn;
    }
}