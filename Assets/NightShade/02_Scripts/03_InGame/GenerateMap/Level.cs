
using System.Collections.Generic;
using UnityEngine;

public static class Level
{
    public static float Height = 500f;      // ����
    public static float Width = 500f;       // �ʺ�
    public static float Scale = 1.0f;       // ũ��
    public static float IconScale = .1f;    // ������ ũ��
    public static float Padding = .01f;     // �е�
    public static float RoomGenerationChance = .5f;     // 0 ~ 1 ������ ���� ���ϴ� Ȯ��(����: 50%) >> 0 : 100%, 1 : 0%
    public static int RoomLimit = 5;      // ���� ������ ���� �� ������ ���� �ִ� ũ��

    public static Sprite DefaultRoomIcon = null;    // �⺻���� �� ������
    public static Sprite TreasureRoonIcon = null;   // ������ �� ������
    public static Sprite BossRoomIcon = null;       // ������ �� ������
    public static Sprite ShopRoomIcon = null;       // ���� �� ������
    public static Sprite CurrentRoomIcon = null;    // �÷��̾ ��ġ�� �� ������
    public static Sprite UnExploredRoomIcon = null; // �÷��̾ �������� ���� �� ������

    public static List<Room> Rooms = new List<Room>();  // ������ ���� ��Ƶδ� ����Ʈ
    public static Room CurrentRoom;                     // ������ �� �� �÷��̾ ��ġ�� ��
}
