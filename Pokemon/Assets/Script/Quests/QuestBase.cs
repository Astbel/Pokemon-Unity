using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*任務是可以設定的這邊是用靜態的方法來去定義任務*/

[CreateAssetMenu(menuName = "Quests/Create a new quest")]
public class QuestBase : ScriptableObject
{
    [SerializeField] string name;
    [SerializeField] string description;

    [SerializeField] Dialog startDialogue;
    [SerializeField] Dialog inProgressDialogue;
    [SerializeField] Dialog completeDialogue;

    [SerializeField] itemBase requiredItem;
    [SerializeField] itemBase rewardItem;

    public string Name => name;
    public string Description => description;
    public Dialog StartDialogue => startDialogue;
    /*如果完成對話小於0則default讓npc獎開始對話*/
    public Dialog InProgressDialogue => inProgressDialogue?.Lines?.Count > 0 ? inProgressDialogue : startDialogue;
    public Dialog CompleteDialogue => completeDialogue;

    public itemBase RequiredItem => requiredItem;
    public itemBase RewardItem => rewardItem;


}
