using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
[CreateAssetMenu(fileName = "Item", menuName = "CreateItem")]
public class Item : ScriptableObject
{

	public enum KindOfItem
	{
		Weapon,
		Armor,
		UseItem,
		Trasure,
	}

	//　アイテムの種類
	[SerializeField]
	private KindOfItem kindOfItem;
	//　アイテムの名前
	[SerializeField]
	private string itemName;
	//　アイテムの情報
	[SerializeField]
	private string information;
	//　アイテムの価格
	[SerializeField]
	private int price;

	// 新たに生成する時:originがスクリプタブル:コンストラクタ
	public Item(Item origin)
	{
		this.kindOfItem = origin.kindOfItem;
		this.itemName = origin.itemName;
		this.information = origin.information;
		this.price = origin.price;
	}

	public KindOfItem GetKindOfItem()
	{
		return kindOfItem;
	}
	public string GetItemName()
	{
		return itemName;
	}

	public string GetInformation()
	{
		return information;
	}

	public int GetPrice()
	{
		return price;
	}
}
