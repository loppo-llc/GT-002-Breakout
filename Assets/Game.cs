using GameCanvas;
using Unity.Mathematics;

sealed class Game : GameBase
{
    System.Action m_Update;
    GcCircle m_Ball;
    float2 m_BallDir;
    GcAABB m_Puddle;
    GcAABB m_WallL, m_WallT, m_WallR, m_WallB;
    int m_BrokenCount;
    bool[] m_Blocks;

    public override System.Collections.IEnumerator Entry()
    {
        // 変数の初期化
        m_Update = new System.Action(Title);
        m_Blocks = new bool[12 * 8];
        m_WallL = GcAABB.XYWH(-1, 0, 1, gc.CanvasHeight);
        m_WallT = GcAABB.XYWH(0, -1, gc.CanvasWidth, 1);
        m_WallR = GcAABB.XYWH(gc.CanvasWidth, 0, 1, gc.CanvasHeight);
        m_WallB = GcAABB.XYWH(0, gc.CanvasHeight, gc.CanvasWidth, 1);

        // キャンバスの大きさを設定
        gc.ChangeCanvasSize(720, 1280);

        // ゲームループ
        while (true)
        {
            // 画面を白で塗りつぶす
            gc.ClearScreen();

            // 更新関数を呼ぶ
            m_Update.Invoke();

            // 次のフレームを待つ
            yield return null;
        }
    }

    /// <summary>
    /// タイトル画面
    /// </summary>
    private void Title()
    {
        // 画像を描画する
        gc.DrawImage(GcImage.Tofu, -34, 320);

        // 文字を描画する
        gc.SetColor(0, 0, 0);
        gc.SetFontSize(48);
        gc.SetStringAnchor(GcAnchor.UpperCenter);
        gc.DrawString("豆腐くずし", 360, 490);

        gc.SetFontSize(24);
        gc.DrawString("画面をタップして開始", 360, 990);

        // 画面から指を離した瞬間なら
        if (gc.IsTouchEnded())
        {
            // すべてのブロックを有効にする
            m_BrokenCount = 0;
            for (var i = 0; i < m_Blocks.Length; i++) m_Blocks[i] = true;

            // パドルを画面中央に
            m_Puddle = GcAABB.XYWH(270, 1080, 180, 20);

            // ボールをパドルの少し上から放つ
            m_Ball = new GcCircle(380, 1000, 10);
            m_BallDir = new float2(1, -1);

            // プレイ画面に移行
            m_Update = new System.Action(Play);
        }
    }

    /// <summary>
    /// プレイ画面
    /// </summary>
    private void Play()
    {
        // 画面に指が触れているなら
        if (gc.TryGetPointerEvent(0, out var e))
        {
            // パドルより左側をタッチしているなら
            if (e.Point.x < m_Puddle.Center.x)
            {
                // パドルをさらに左に移動する。ただし左端まで
                m_Puddle.Center.x = GcMath.Max(m_Puddle.Center.x - 5, 90);
            }

            // パドルより右側をタッチしているなら
            if (e.Point.x > m_Puddle.Center.x)
            {
                // パドルをさらに右に移動する。ただし右端まで
                m_Puddle.Center.x = GcMath.Min(m_Puddle.Center.x + 5, 630);
            }
        }

        // ボールの速度と移動距離を計算
        var ballSpeed = 5 + (m_BrokenCount) / 10; // ブロックを崩すたびに速度が増していく
        var ballDelta = m_BallDir * ballSpeed;

        // パドルと接触したら
        if (gc.SweepTest(m_Puddle, m_Ball.Position, ballDelta, out var result))
        {
            // 跳ね返る
            m_Ball.Position = result.PositionOnHit;
            result.CalcReflect(out m_BallDir, out var reflectPos);
            ballDelta = reflectPos - m_Ball.Position;
        }

        for (var i = 0; i < 12; i++)
        {
            for (var j = 0; j < 8; j++)
            {
                // 無効なブロックはスキップ
                if (!m_Blocks[i + 12 * j]) continue;

                // 有効なブロックを描画する
                var block = GcAABB.XYWH(30 + 55 * i, 30 + 25 * j, 50, 20);
                gc.SetColor(gc.ColorGray);
                gc.FillRect(block);

                // ブロックと接触したら
                if (gc.SweepTest(block, m_Ball.Position, ballDelta, out result))
                {
                    // ブロックを消す
                    m_Blocks[i + 12 * j] = false;

                    // 破壊カウントを加算
                    m_BrokenCount++;

                    // すべてのブロックを破壊したら
                    if (m_BrokenCount == m_Blocks.Length)
                    {
                        // ゲームクリア
                        m_Update = new System.Action(Result);
                    }

                    // 跳ね返る
                    m_Ball.Position = result.PositionOnHit;
                    result.CalcReflect(out m_BallDir, out var reflectPos);
                    ballDelta = reflectPos - m_Ball.Position;
                }
            }
        }

        // ボールが左端に到達したら
        if (gc.SweepTest(m_WallL, m_Ball.Position, ballDelta, out result))
        {
            // 跳ね返る
            m_Ball.Position = result.PositionOnHit;
            result.CalcReflect(out m_BallDir, out var reflectPos);
            ballDelta = reflectPos - m_Ball.Position;
        }
        // ボールが上端に到達したら
        if (gc.SweepTest(m_WallT, m_Ball.Position, ballDelta, out result))
        {
            // 跳ね返る
            m_Ball.Position = result.PositionOnHit;
            result.CalcReflect(out m_BallDir, out var reflectPos);
            ballDelta = reflectPos - m_Ball.Position;
        }
        // ボールが右端に到達したら
        if (gc.SweepTest(m_WallR, m_Ball.Position, ballDelta, out result))
        {
            // 跳ね返る
            m_Ball.Position = result.PositionOnHit;
            result.CalcReflect(out m_BallDir, out var reflectPos);
            ballDelta = reflectPos - m_Ball.Position;
        }
        // ボールが下端に到達したら
        if (gc.SweepTest(m_WallB, m_Ball.Position, ballDelta, out result))
        {
            // ゲームオーバー
            m_Update = new System.Action(Result);
        }

        // ボールの移動
        m_Ball.Position += ballDelta;

        // ボールを描画する
        gc.SetColor(gc.ColorRed);
        gc.FillCircle(m_Ball);

        // パドルを描画する
        gc.SetColor(gc.ColorBlack);
        gc.FillRect(m_Puddle);

        // 破壊した数を描画する
        gc.SetFontSize(64);
        gc.SetStringAnchor(GcAnchor.UpperRight);
        gc.DrawString($"{m_Blocks.Length - m_BrokenCount:00}", 680, 1200);
    }

    private void Result()
    {
        // 文字を描画する
        gc.SetColor(0, 0, 0);
        gc.SetFontSize(48);
        gc.SetStringAnchor(GcAnchor.UpperCenter);
        gc.DrawString("終了", 360, 560);

        gc.SetFontSize(36);
        gc.DrawString($"くずした豆腐の数： {m_BrokenCount:00} 個", 360, 660);

        gc.SetFontSize(24);
        gc.DrawString("画面をタップしてタイトルに戻る", 360, 740);

        // 画面から指を離した瞬間なら
        if (gc.IsTouchEnded())
        {
            // タイトル画面に移行
            m_Update = new System.Action(Title);
        }
    }
}
