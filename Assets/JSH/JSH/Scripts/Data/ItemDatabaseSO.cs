using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemDatabaseSO", menuName = "Database/ItemDatabaseSO")]
public class ItemDatabaseSO : ScriptableObject
{
    public List<ItemDataSO> items;
}