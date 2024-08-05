//System
using System.Collections;
using System.Collections.Generic;

//UnityEngine
using UnityEngine;
using UnityEngine.UI;

public class GenertateLevel : MonoBehaviour
{
    public Sprite emptyRoom;    // �⺻���� �� �̹���
    public Sprite currentRoom;  // �÷��̾ ��ġ�� �� �̹���
    public Sprite unExplored;   // �÷��̾ �������� ���� �� �̹���
    public Sprite bossRoom;     // ���� �� �̹���
    public Sprite shopRoom;     // ���� �� �̹���
    public Sprite treasureRoom; // ������ �� �̹���

    private int failSafe = 0;   // �� ���� ���� ������ ����
    private bool reGenerating = false;

    private void Awake()
    {
        Init();
    }

    private void Start()
    {
        // ó�� �÷��̾ ���� ���� ����
        Room startRoom = new Room();
        startRoom.Location = new Vector2(0, 0);
        startRoom.RoomImage = Level.CurrentRoomIcon;

        // ���� ����
        Generate(startRoom);
    }

    private void Update()
    {
        // Tab�� ������ �� ���� �ʱ�ȭ ��Ű�� �ٽ� ����
        if(Input.GetKeyDown(KeyCode.Tab) && !reGenerating)
        {
            reGenerating = true;
            Invoke(nameof(StopRegenerating), 1);

            for(int i = transform.childCount - 1; i >= 0; i--)
            {
                Transform child = transform.GetChild(i);
                Destroy(child.gameObject);
            }

            Level.Rooms.Clear();

            Start();
        }
    }

    /// <summary>
    /// 'Level' ���� Ŭ������ �ִ� �������� �� �������� ����
    /// </summary>
    private void Init()
    {
        Level.DefaultRoomIcon = emptyRoom;
        Level.CurrentRoomIcon = currentRoom;
        Level.UnExploredRoomIcon = unExplored;
        Level.BossRoomIcon = bossRoom;
        Level.ShopRoomIcon = shopRoom;
        Level.TreasureRoonIcon = treasureRoom;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="room">��</param>
    private void DrawRoomOnMap(Room room)
    {
        // ������Ʈ�� ����
        GameObject mapTile = new GameObject("MapTile");     
        
        // �̹��� ������Ʈ�� �ִ´�
        Image roomImage = mapTile.AddComponent<Image>();
        roomImage.sprite = room.RoomImage;
        RectTransform rect = roomImage.GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(Level.Height, Level.Width) * Level.IconScale;
        rect.position = room.Location * (Level.IconScale * Level.Height * Level.Scale + (Level.Padding * Level.Height * Level.Scale));
        roomImage.transform.SetParent(transform, false);

        Level.Rooms.Add(room);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="vector"></param>
    /// <returns></returns>
    private bool CheckIfRoomExists(Vector2 vector)
    {
        // Exists: ��ȣ �ȿ� ���ǹ��� ���� True�Ǵ� False�� ��ȯ
        // ���� ��ġ�� vector��� �޼ҵ��� ���ڰ��� ������ ���� True �Ǵ� False�� ��ȯ
        return (Level.Rooms.Exists(x => x.Location == vector));
    }

    /// <summary>
    /// ���� �Ǿ��� �� �ֺ��� ���� �Ǳ� �� ������ �����ϰ� �� �ٸ� ���� �����ϴ��� Ȯ�� 
    /// </summary>
    /// <param name="vector">��ǥ</param>
    /// <param name="direction">����</param>
    /// <returns></returns>
    private bool CheckIfRoomsAroundGeneratedRoom(Vector2 vector, string direction)
    {
        switch(direction)
        {
            // �����ʿ��� �������� ���� �����ȴٰ� ����
            case "Right":
                {
                    // ����, ��, �Ʒ��� Ȯ�� 
                    if (Level.Rooms.Exists(x => x.Location == new Vector2(vector.x - 1, vector.y)) ||
                        Level.Rooms.Exists(x => x.Location == new Vector2(vector.x, vector.y + 1)) ||
                        Level.Rooms.Exists(x => x.Location == new Vector2(vector.x, vector.y - 1)))
                    {
                        return true;
                    }
                    break;
                }
            // ���ʿ��� ���������� ���� �����ȴٰ� ����
            case "Left":
                {
                    // ������, ��, �Ʒ��� Ȯ�� 
                    if (Level.Rooms.Exists(x => x.Location == new Vector2(vector.x + 1, vector.y)) ||
                        Level.Rooms.Exists(x => x.Location == new Vector2(vector.x, vector.y + 1)) ||
                        Level.Rooms.Exists(x => x.Location == new Vector2(vector.x, vector.y - 1)))
                    {
                        return true;
                    }
                    break;
                }
            // ���ʿ��� �Ʒ������� ���� �����ȴٰ� ����
            case "Up":
                {
                    // ����, ������, �Ʒ��� Ȯ�� 
                    if (Level.Rooms.Exists(x => x.Location == new Vector2(vector.x - 1, vector.y)) ||
                        Level.Rooms.Exists(x => x.Location == new Vector2(vector.x + 1, vector.y)) ||
                        Level.Rooms.Exists(x => x.Location == new Vector2(vector.x, vector.y - 1)))
                    {
                        return true;
                    }
                    break;
                }
            // �Ʒ��ʿ��� �������� ���� �����ȴٰ� ����
            case "Down":
                {
                    // ����, ������, ���� Ȯ�� 
                    if (Level.Rooms.Exists(x => x.Location == new Vector2(vector.x - 1, vector.y)) ||
                        Level.Rooms.Exists(x => x.Location == new Vector2(vector.x + 1, vector.y)) ||
                        Level.Rooms.Exists(x => x.Location == new Vector2(vector.x, vector.y + 1)))
                    {
                        return true;
                    }
                    break;
                }
        }

        return false;
    }

    /// <summary>
    /// ���� ����
    /// </summary>
    /// <param name="room">��</param>
    private void Generate(Room room)
    {
        failSafe++;
        if (failSafe > 50) return;

        DrawRoomOnMap(room);

        // ���ʻ���
        if (Random.value > Level.RoomGenerationChance)
        {
            Room newRoom = new Room();
            newRoom.Location = new Vector2(-1, 0) + room.Location;
            newRoom.RoomImage = Level.DefaultRoomIcon;

            if (!CheckIfRoomExists(newRoom.Location))
            {
                if (!CheckIfRoomsAroundGeneratedRoom(newRoom.Location, "Right"))
                {
                    if (Mathf.Abs(newRoom.Location.x) < Level.RoomLimit && Mathf.Abs(newRoom.Location.y) < Level.RoomLimit)
                    {
                        Generate(newRoom);
                    }
                }
            }           
        }

        // �����ʻ���
        if (Random.value > Level.RoomGenerationChance)
        {
            Room newRoom = new Room();
            newRoom.Location = new Vector2(1, 0) + room.Location;
            newRoom.RoomImage = Level.DefaultRoomIcon;

            if (!CheckIfRoomExists(newRoom.Location))
            {
                if (!CheckIfRoomsAroundGeneratedRoom(newRoom.Location, "Left"))
                {
                    if (Mathf.Abs(newRoom.Location.x) < Level.RoomLimit && Mathf.Abs(newRoom.Location.y) < Level.RoomLimit)
                    {
                        Generate(newRoom);
                    }
                }
            }
        }

        // ���ʻ���
        if (Random.value > Level.RoomGenerationChance)
        {
            Room newRoom = new Room();
            newRoom.Location = new Vector2(0, 1) + room.Location;
            newRoom.RoomImage = Level.DefaultRoomIcon;

            if (!CheckIfRoomExists(newRoom.Location))
            {
                if (!CheckIfRoomsAroundGeneratedRoom(newRoom.Location, "Down"))
                {
                    if (Mathf.Abs(newRoom.Location.x) < Level.RoomLimit && Mathf.Abs(newRoom.Location.y) < Level.RoomLimit)
                    {
                        Generate(newRoom);
                    }
                }
            }
        }

        // �Ʒ��ʻ���
        if (Random.value > Level.RoomGenerationChance)
        {
            Room newRoom = new Room();
            newRoom.Location = new Vector2(0, -1) + room.Location;
            newRoom.RoomImage = Level.DefaultRoomIcon;

            if (!CheckIfRoomExists(newRoom.Location))
            {
                if (!CheckIfRoomsAroundGeneratedRoom(newRoom.Location, "Up"))
                {
                    if (Mathf.Abs(newRoom.Location.x) < Level.RoomLimit && Mathf.Abs(newRoom.Location.y) < Level.RoomLimit)
                    {
                        Generate(newRoom);
                    }
                }
            }
        }

        GenerateBossRoom();
    }

    /// <summary>
    /// 
    /// </summary>
    private void StopRegenerating()
    {
        reGenerating = false;
    }

    #region Rooms Generation
    /// <summary>
    /// 
    /// </summary>
    private void GenerateBossRoom()
    {
        float maxNumber = 0;
        Vector2 fathestRoom = Vector2.zero;

        foreach(Room r in Level.Rooms)
        {
            if(Mathf.Abs(r.Location.x) + Mathf.Abs(r.Location.y) >= maxNumber)
            {
                maxNumber = Mathf.Abs(r.Location.x) + Mathf.Abs(r.Location.y);
                fathestRoom = r.Location;
            }
        }

        Room bossRoom = new Room();
        bossRoom.RoomImage = Level.BossRoomIcon;
        bossRoom.RoomNumber = 3;

        // ����
        if (!CheckIfRoomExists(fathestRoom))
            if (!CheckIfRoomsAroundGeneratedRoom(fathestRoom + new Vector2(-1, 0), "Right"))
            {
                bossRoom.Location = fathestRoom + new Vector2(-1, 0);
            }

        // ������
        else if (!CheckIfRoomExists(fathestRoom))
            if (!CheckIfRoomsAroundGeneratedRoom(fathestRoom + new Vector2(1, 0), "Left"))
            {
                bossRoom.Location = fathestRoom + new Vector2(1, 0);
            }

        // ����
        else if (!CheckIfRoomExists(fathestRoom))
            if (!CheckIfRoomsAroundGeneratedRoom(fathestRoom + new Vector2(0, 1), "Down"))
            {
                bossRoom.Location = fathestRoom + new Vector2(0, 1);
            }

        // �Ʒ���
        else if (!CheckIfRoomExists(fathestRoom))
            if (!CheckIfRoomsAroundGeneratedRoom(fathestRoom + new Vector2(0, -1), "Up"))
            {
                bossRoom.Location = fathestRoom + new Vector2(0, -1);
            }

        DrawRoomOnMap(bossRoom);

    }
    #endregion
}
