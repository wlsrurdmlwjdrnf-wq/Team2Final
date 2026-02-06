using System.Text.Json;
using System.Text.Json.Serialization;
using TeamProjectServer.Models;

namespace TeamProjectServer.Services
{
    public static class DataManager
    {
        //데이터 컨테이너
        private static Dictionary<string, Dictionary<int, BaseData>> _table = new();
        private static List<InventorySlot> _inventoryInit = new();

        //저장되는 json 폴더 경로
        private static readonly string FilePath = Path.Combine(Directory.GetCurrentDirectory(), "GameData");
        private static readonly string FileName = "InitData.json";

        // 시트 데이터 동기화
        public static async Task SyncSheet(string url)
        {
            using var client = new HttpClient();
            //주소에서 받아온 json
            var json = await client.GetStringAsync(url);

            if (!Directory.Exists(FilePath))
            {
                Directory.CreateDirectory(FilePath);
            }
            await File.WriteAllTextAsync(Path.Combine(FilePath, FileName), json);

            //전체 json을(기본데이터) 서버 GameData 폴더에 저장
            LoadInitData(json);

        }


        private static void LoadInitData(string json)
        {

            //대소문자 무시
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                Converters = { new JsonStringEnumConverter() }
            };

            //전체 json을 탭으로 나눔
            var root = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(json, options);


            if (root != null && root.TryGetValue("data", out var dataElement))
            {
                var rawData = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(dataElement.GetRawText(), options);

                if (rawData != null)
                {
                    // 기존 데이터 정리
                    _table.Clear();
                    _inventoryInit.Clear();

                    //각 탭에 맞는 json 데이터 저장
                    LoadTable<PlayerInit>(rawData, options, false);
                    LoadTable<Accessory>(rawData, options, true);
                    LoadTable<Artifact>(rawData, options, true);
                    LoadTable<Skill>(rawData, options, true);
                    LoadTable<Stage>(rawData, options, false);
                    LoadTable<Weapon>(rawData, options, true);
                }
            }
        }

        private static void LoadTable<T>(Dictionary<string, JsonElement> rawData, JsonSerializerOptions options, bool invendata) where T : BaseData
        {
            string key = typeof(T).Name;
            //매칭되는 탭이름 있는지 확인후 데이터 주입
            if (rawData.TryGetValue(key, out var element))
            {
                //각 탭의 모든 행을 리스트로 저장 후 ID를 키값으로 딕셔너리로 변환
                var list = JsonSerializer.Deserialize<List<T>>(element.GetRawText(), options);
                _table[key] = list.ToDictionary(data => data.ID, data => (BaseData)data);

                if (invendata) // 도감 데이터는 인벤토리 초기화 설정
                {
                    foreach (var item in list)
                    {
                        _inventoryInit.Add(new InventorySlot
                        {
                            ID = item.ID,
                            Stack = 0,
                            IsLocked = true,
                        });
                    }
                }
            }
        }

        //초기화 호출 함수
        public static void Initialize()
        {
            string fullPath = Path.Combine(FilePath, FileName);
            if (File.Exists(fullPath)) LoadInitData(File.ReadAllText(fullPath));
        }

        //초기화 인벤토리 데이터 반환 함수
        public static List<InventorySlot> GetInitInventoryData()
        {
            return _inventoryInit.Select(x => new InventorySlot
            {
                ID = x.ID,
                Stack = x.Stack,
                IsLocked = x.IsLocked,
            }).ToList();
        }

        //데이터 조회 함수
        public static T Get<T>(int id) where T : BaseData
        {
            //basedata 자식의 이름을 키로 설정
            string key = typeof(T).Name;

            //테이블에 해당 이름이 있는지 && ID가 있는지
            if (_table.TryGetValue(key, out var row) && row.TryGetValue(id, out var data))
            {
                //자식 타입으로 리턴
                return (T)data;
            }
            else
            {
                return null;
            }
        }
    }
}
