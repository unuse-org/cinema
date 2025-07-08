// using UnityEngine;
// using System.Collections.Generic;

// public class MovieDataLoader : MonoBehaviour
// {
//     [SerializeField] private TextAsset jsonFile;

//     [System.Serializable]
//     public class Movie
//     {
//         public int movieIndex;
//         public string movieName;
//         public float scoreStartTime;
//         public float scoreEndTime;
//         public bool scoreCondition;
//     }

//     [System.Serializable]
//     public class MovieList
//     {
//         public List<Movie> movies;
//     }

//     private MovieList movieList;

//     public float scoreStartTime { get; private set; }
//     public float scoreEndTime { get; private set; }
//     public bool scoreCondition { get; private set; }

//     void Awake()
//     {
//         if (jsonFile == null)
//         {
//             Debug.LogError("jsonFile が設定されていません。");
//             return;
//         }

//         // JSON読み込み
//         movieList = JsonUtility.FromJson<MovieList>(jsonFile.text);

//         if (movieList == null || movieList.movies == null)
//         {
//             Debug.LogError("JSONの読み込みまたはパースに失敗しました。");
//             return;
//         }
//     }

//         /// <summary>
//     /// 映画データに基づき、再生時間と条件が一致しているかをチェックします。
//     /// </summary>
//     /// <param name="currentTime">現在の動画再生時間（秒）</param>
//     /// <param name="inputCondition">外部入力の条件値（true/false）</param>
//     /// <returns>条件が一致すれば true、不一致なら false</returns>
//     public bool CheckMovieCondition(float currentTime, bool inputCondition)
//     {
//         int movieIndex = PlayerPrefs.GetInt("movie", 0);

//         foreach (var movie in movieList.movies)
//         {
//             if (movie.movieIndex == movieIndex)
//             {
//                 bool isInTimeRange = currentTime >= movie.scoreStartTime && currentTime <= movie.scoreEndTime;
//                 bool isConditionMatched = movie.scoreCondition == inputCondition;

//                 if (isInTimeRange && isConditionMatched)
//                 {
//                     //Debug.Log($"✅ 条件一致: {movie.movieName} | 時間: {currentTime:F2}s | 条件: {inputCondition}");
//                     return true;
//                 }
//                 else
//                 {
//                     //Debug.Log($"⛔ 条件不一致: {movie.movieName} | 時間: {currentTime:F2}s | 条件: {inputCondition} " +
//                         //$"(期待値: {movie.scoreCondition}, 範囲: {movie.scoreStartTime}〜{movie.scoreEndTime})");
//                     return false;
//                 }
//             }
//         }

//         Debug.LogWarning($"⚠️ movieIndex {movieIndex} に一致する映画データが見つかりません。");
//         return false;
//     }

// }
