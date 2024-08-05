
using System.Collections.Generic;
using UnityEngine;

public static class Level
{
    public static float Height = 500f;      // 높이
    public static float Width = 500f;       // 너비
    public static float Scale = 1.0f;       // 크기
    public static float IconScale = .1f;    // 아이콘 크기
    public static float Padding = .01f;     // 패딩
    public static float RoomGenerationChance = .5f;     // 0 ~ 1 사이의 값중 원하는 확률(지정: 50%) >> 0 : 100%, 1 : 0%
    public static int RoomLimit = 5;      // 방의 생성이 도달 할 가능한 맵의 최대 크기

    public static Sprite DefaultRoomIcon = null;    // 기본적인 방 아이콘
    public static Sprite TreasureRoonIcon = null;   // 보물방 방 아이콘
    public static Sprite BossRoomIcon = null;       // 보스방 방 아이콘
    public static Sprite ShopRoomIcon = null;       // 상점 방 아이콘
    public static Sprite CurrentRoomIcon = null;    // 플레이어가 위치한 방 아이콘
    public static Sprite UnExploredRoomIcon = null; // 플레이어가 도달하지 못한 방 아이콘

    public static List<Room> Rooms = new List<Room>();  // 생성된 방을 모아두는 리스트
    public static Room CurrentRoom;                     // 생성된 방 중 플레이어가 위치한 방
}
