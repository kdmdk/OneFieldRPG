using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
[CreateAssetMenu(fileName = "Actor", menuName = "CreateActor")]
public class ActorStatus : ScriptableObject
{
	public enum Alignment
	{
		good,
		neutral,
		evil,
	}

	//　アクターの属性
	[SerializeField]
	private Alignment alignment;
	//　アクターの名前
	[SerializeField]
	private string actorName;
	//　アクターの情報
	[SerializeField]
	private string information;
	//　アクターの攻撃力
	[SerializeField]
	private int attack;
	//　アクターの防御力
	[SerializeField]
	private int defense;
	//	アクターのレベル
	[SerializeField]
	private int level;


	public Alignment GetAlignment()
    {
		return alignment;
    }
	public string GetActorName()
	{
		return actorName;
	}
	public string GetInfomation()
	{
		return information;
	}
	public int GetAttack()
	{
		return attack;
	}
	public int GetDefense()
	{
		return defense;
	}
	public int GetLevel()
	{
		return level;
	}
}