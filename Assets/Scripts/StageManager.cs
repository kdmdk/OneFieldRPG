using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class StageManager : MonoBehaviour
{
    public enum TILE_TYPE
    {
        SEA,        //0　海
        GROUND,     //1　大地
        FOREST,     //2　森
        CAVE,       //3　洞窟
        TOWER,      //4　塔
        CASTLE,     //5　城
        MOUNTAIN,   //6  山
        HOUSE,      //7  家
        DUNGEON,    //8  ダンジョン
        PLAYER,     //9　プレイヤー
    }
    public TextAsset[] stageFiles;  // ステージ構造が記述されたテキストファイル
    TILE_TYPE[,] tileTable;　　// タイル情報を管理する二次元配列
    float tileSize;　　　　　　// タイルのサイズ
    int BlockCount;      // ブロックの数
    Vector2 centerPosition;

    public GameObject[] prefabs;  // ゲームオブジェクトをプレハブしリスト化
    public PlayerManager player; // playermanager
    public MessageManager messageManager;

    public Dictionary<GameObject, Vector2Int> moveObjPositionOnTile = new Dictionary<GameObject, Vector2Int>();


    public void LoadTileData(int num)
    {
        // タイルの列数を計算
        string[] lines = stageFiles[num].text.Split(new[] { '\n', '\r' }, System.StringSplitOptions.RemoveEmptyEntries);
        // [X] 列の長さを取得(lines[0]で1行取得してその中のsplitを,区切りで分けた場合の長さ)
        int columns = lines[0].Split(new[] { ',' }).Length;
        // [Y] 行の長さを取得
        int rows = lines.Length;
        // tileTableのコード[X,Y] = [列,行]を代入
        tileTable = new TILE_TYPE[columns, rows];
        for (int y = 0; y < rows; y++)
        {
            string[] values = lines[y].Split(new[] { ',' });
            for (int x = 0; x < columns; x++)
            {
                //tileTable[x, y] = (TILE_TYPE)int.Parse(values[x]);
                switch (values[x])
                {
                    //BlockTypeが追加されたらcaseを対応させていく
                    case "0":
                        tileTable[x, y] = TILE_TYPE.SEA;
                        break;
                    case "1":
                        tileTable[x, y] = TILE_TYPE.GROUND;
                        break;
                    case "2":
                        tileTable[x, y] = TILE_TYPE.FOREST;
                        break;
                    case "3":
                        tileTable[x, y] = TILE_TYPE.CAVE;
                        break;
                    case "4":
                        tileTable[x, y] = TILE_TYPE.TOWER;
                        break;
                    case "5":
                        tileTable[x, y] = TILE_TYPE.CASTLE;
                        break;
                    case "6":
                        tileTable[x, y] = TILE_TYPE.MOUNTAIN;
                        break;
                    case "7":
                        tileTable[x, y] = TILE_TYPE.HOUSE;
                        break;
                    case "8":
                        tileTable[x, y] = TILE_TYPE.DUNGEON;
                        break;
                    case "9":
                        tileTable[x, y] = TILE_TYPE.PLAYER;
                        break;
                }
            }
        }
    }

        //tileTableを使ってタイルを生成＆配置する
        public void CreateStage()
        {
            tileSize = prefabs[0].GetComponent<SpriteRenderer>().bounds.size.x;
            centerPosition.x = (tileTable.GetLength(0) / 2) * tileSize;
            centerPosition.y = (tileTable.GetLength(1) / 2) * tileSize;

            for (int y = 0; y < tileTable.GetLength(1); y++)
            {

                for (int x = 0; x < tileTable.GetLength(0); x++)
                {


                    Vector2Int position = new Vector2Int(x, y);
                    //　groundを予め敷き詰める
                    GameObject ground = Instantiate(prefabs[(int)TILE_TYPE.GROUND]);
                    ground.transform.position = GetScreenPositionFromTileTable(position);

                    //　タイルを配置する⇒この時PLAYERからPlayerManagerを取得する
                    TILE_TYPE tileType = tileTable[x, y];
                    GameObject obj = Instantiate(prefabs[(int)tileType]);
                    obj.transform.position = GetScreenPositionFromTileTable(position);

                if (tileType == TILE_TYPE.PLAYER)
                { 
                    player = obj.GetComponent<PlayerManager>();
                    player.name = "Player";
                    moveObjPositionOnTile.Add(obj, position);
                }

                if (tileType == TILE_TYPE.SEA)

                {
                    moveObjPositionOnTile.Add(obj, position);
                }

                if (tileType == TILE_TYPE.FOREST)

                {
                    moveObjPositionOnTile.Add(obj, position);
                }
                if (tileType == TILE_TYPE.CAVE)

                {
                    moveObjPositionOnTile.Add(obj, position);
                }
                if (tileType == TILE_TYPE.TOWER)

                {
                    moveObjPositionOnTile.Add(obj, position);
                }
                if (tileType == TILE_TYPE.CASTLE)

                {
                    
                    moveObjPositionOnTile.Add(obj, position);
                }
                if (tileType == TILE_TYPE.MOUNTAIN)

                {
                    moveObjPositionOnTile.Add(obj, position);
                }
                if (tileType == TILE_TYPE.HOUSE)

                {
                    moveObjPositionOnTile.Add(obj, position);
                }
                if (tileType == TILE_TYPE.DUNGEON)

                {
                    moveObjPositionOnTile.Add(obj, position);
                }
            }

        }

    }
    //GetScreenPositionFromTileTable();の関数を使ってタイルを敷き詰めるサイズを算出
    //GetScreenPositionFromTileTableが画面内のサイズの比率を定義している
    //描画の反転を修正するには、y座標を-にすればいい。
    public Vector2 GetScreenPositionFromTileTable(Vector2Int position)
    {
        return new Vector2(position.x * tileSize - centerPosition.x, -(position.y * tileSize - centerPosition.y));
    }

    // 指定された位置のタイルがSEAなら true を返す
    public bool IsSea(Vector2Int position)
    {
        if (tileTable[position.x, position.y] == TILE_TYPE.SEA)
        {
            return true;
        }
        return false;
    }

    // 指定された位置のタイルがMOUNTAINなら true を返す
    public bool IsMountain(Vector2Int position)
    {
        if (tileTable[position.x, position.y] == TILE_TYPE.MOUNTAIN)
        {
            return true;
        }
        return false;
    }
    public bool IsCastle(Vector2Int position)
    {
        if (tileTable[position.x, position.y] == TILE_TYPE.CASTLE)
        {
            return true;
        }
        return false;
    }
    public bool IsGround(Vector2Int position)
    {
        if (tileTable[position.x, position.y] == TILE_TYPE.GROUND)
        {
            return true;
        }
        return false;
    }
    public bool IsForest(Vector2Int position)
    {
        if (tileTable[position.x, position.y] == TILE_TYPE.FOREST)
        {
            return true;
        }
        return false;
    }
    public bool IsCave(Vector2Int position)
    {
        if (tileTable[position.x, position.y] == TILE_TYPE.CAVE)
        {
            return true;
        }
        return false;
    }
    public bool IsHouse(Vector2Int position)
    {
        if (tileTable[position.x, position.y] == TILE_TYPE.HOUSE)
        {
            return true;
        }
        return false;
    }
    public bool IsTower(Vector2Int position)
    {
        if (tileTable[position.x, position.y] == TILE_TYPE.TOWER)
        {
            return true;
        }
        return false;
    }
    public bool IsDungeon(Vector2Int position)
    {
        if (tileTable[position.x, position.y] == TILE_TYPE.DUNGEON)
        {
            return true;
        }
        return false;
    }
    public bool IsPlayerInitialPosition(Vector2Int position)
    {
        if (tileTable[position.x, position.y] == TILE_TYPE.PLAYER)
        {
            return true;
        }
        return false;
    }
    /*
    public void UpdateTileTableForPlayer(Vector2Int currentPosition, Vector2Int nextPosition)
    {
        //tileTableの更新
        //次にPlayerが置かれる場所をPLAYERとする(ただし、BLOCK_POINTならPLAYER_ON_POINTにする)
        if (tileTable[nextPosition.x, nextPosition.y] == TILE_TYPE.BLOCK_POINT)
        {
            tileTable[nextPosition.x, nextPosition.y] = TILE_TYPE.PLAYER_ON_POINT;
        }
        else
        {
            tileTable[nextPosition.x, nextPosition.y] = TILE_TYPE.PLAYER;
        }

        //現在の場所をGROUNDとする
        if (tileTable[currentPosition.x, currentPosition.y] == TILE_TYPE.PLAYER_ON_POINT)
        {
            tileTable[currentPosition.x, currentPosition.y] = TILE_TYPE.BLOCK_POINT;
        }
        else
        {
            tileTable[currentPosition.x, currentPosition.y] = TILE_TYPE.GROUND;
        }
    }
    */
}


