using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StoreFishList : MonoBehaviour
{
    // 所持している魚データのリスト
    public static List<FishData> fishDataList = new List<FishData>();
    private List<Dropdown> dropdowns;
    [SerializeField]
    private GameObject InputField;

    // Start is called before the first frame update
    void Awake()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnEnable()
    {
        Debug.Log(0);
        fishDataList.Clear();
        Debug.Log(0);
        Player.fish.Sort((a,b) => a.ID - b.ID);
        foreach(FishEntity fish in Player.fish)
        {
            if (fishDataList.Count == 0)
            {
                Debug.Log(1);
                fishDataList.Add(new FishData() { Fish = fish, Image = Resources.Load(string.Format("Usable_Fish/{0}/{0}", fish.Name), typeof(Sprite)) as Sprite, Num = 1 });
                continue;
            }
            Debug.Log(2);
            if (fishDataList[fishDataList.Count - 1].Fish == fish) fishDataList[fishDataList.Count - 1].Num++;
            else fishDataList.Add(new FishData() { Fish = fish, Image = Resources.Load(string.Format("Usable_Fish/{0}/{0}", fish.Name), typeof(Sprite)) as Sprite, Num = 1 });
            Debug.Log(3);
        }

        for(int i = 0; i < fishDataList.Count; i++)
        {
            Debug.Log(4);
            GameObject fishData =  Instantiate(InputField, new Vector3(0,0,0), new Quaternion(0,0,0,0));
            fishData.transform.SetParent(transform);
            fishData.GetComponent<RectTransform>().localPosition = new Vector3(0,0,0);
            fishData.GetComponent<RectTransform>().sizeDelta = new Vector2(0, 10);
            fishData.transform.GetChild(0).GetComponent<Image>().sprite = Resources.Load(string.Format("Usable_Fish/{0}/{0}", fishDataList[i].Fish.Name), typeof(Sprite)) as Sprite;
            fishData.transform.GetChild(1).GetComponent<Text>().text = fishDataList[i].Fish.Name;
            fishData.transform.GetChild(2).GetChild(0).GetChild(1).GetComponent<Text>().text = fishDataList[i].Num.ToString();
            Debug.Log(fishDataList[i].Num.ToString());
        }
    }
}
