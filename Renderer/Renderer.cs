// ↓↓                ↓↓
// ↓↓     FIX ME     ↓↓
using COIS2020.priashabarua0778496.Assignment3;
// ↑↑                ↑↑
// ↑↑                ↑↑

/* ==================================================================================================================
 * Aside from fixing the `using` directive on the first line:
 *
 * YOU DO NOT NEED TO TOUCH THIS FILE!!! THINGS MIGHT BREAK IF YOU DO!!!
 *
 * Of course, you're more than welcome to tinker if you'd like. Really you should just know that it's okay to totally
 * ignore this super-big scary file. :)
 *
 * If something in this file breaks, but you _didn't_ modify it, please let Matt or Sri know.
 * ================================================================================================================== */

namespace COIS2020.StarterCode.Assignment3;

using System.Linq;
using System.Security;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using FontStashSharp;
using TrentCOIS.Tools.Visualization;
using TrentCOIS.Tools.Visualization.Assets;
using TrentCOIS.Tools.Visualization.Input;
using TrentCOIS.Tools.Visualization.Utility;

public sealed class CastleGameRenderer : Renderer<CastleDefender>
{
    /// <summary>
    /// Checks if the given entity has moved off of the screen.
    /// </summary>
    public static bool IsOffScreen(CombatEntity entity) => IsOffScreen(entity.Position);

    /// <summary>
    /// Checks if an entity at the given position is off of the screen.
    /// </summary>
    public static bool IsOffScreen(Vector2 entityPosition)
    {
        Rectangle dest = WorldPosToDest(entityPosition);
        Rectangle main = new(Point.Zero, MainBounds.Size);
        return !dest.Intersects(main);
    }

    /// <summary>
    /// Checks if the given entity has bumped into a wall in the main playfield, and updates its associated direction
    /// vector if it has.
    /// </summary>
    public static void CheckWallCollision(CombatEntity entity, ref Vector2 direction)
    {
        Vector2 pos = entity.Position;
        CheckWallCollision(ref pos, ref direction);
        entity.Position = pos;
    }

    /// <summary>
    /// Checks if the given entity has bumped into a wall in the main playfield, and updates its associated direction
    /// vector if it has.
    /// </summary>
    public static void CheckWallCollision(ref Vector2 pos, ref Vector2 direction)
    {
        var (minX, minY) = FieldTL;
        var (maxX, maxY) = FieldBR;

        if (pos.X < minX)
        {
            pos.X = minX + (minX - pos.X);
            direction = Vector2.Reflect(direction, new Vector2(1, 0));
        }
        else if (pos.X > maxX)
        {
            pos.X = maxX - (pos.X - maxX);
            direction = Vector2.Reflect(direction, new Vector2(-1, 0));
        }

        if (pos.Y < minY)
        {
            pos.Y = minY + (minY - pos.Y);
            direction = Vector2.Reflect(direction, new Vector2(0, 1));
        }
        else if (pos.Y > maxY)
        {
            pos.Y = maxY - (pos.Y - maxY);
            direction = Vector2.Reflect(direction, new Vector2(0, -1));
        }
    }

    /// <summary>
    /// Gets a random color using a pre-seeded RNG for consistency.
    /// </summary>
    public static Color GetRandomColor() => GetRandomColor(0, 360);

    /// <summary>
    /// Gets a random color using a pre-seeded RNG for consistency.
    /// </summary>
    public static Color GetRandomColor(int minHue, int maxHue)
    {
        float h = SeededRNG.Next(minHue, maxHue) / 360f;
        float s = SeededRNG.Next(50, 100) / 100f;
        float l = SeededRNG.Next(38, 69) / 100f;
        return HslToRgb(h, s, l);
    }


    #region Constants/config

    // No guarantees on how well things'll work if you change these...
    private new const int WindowWidth = 1280;
    private new const int WindowHeight = 720;
    private const int PanelMargins = 15;                // Around all panels
    private const int PanelPadding = 6;                 // Only within right-hand panels

    private static Point TileSizeScreen = new(30, 30);  // How big a tile (sprite) appears on screen
    private static Vector2 TileSizeWorld = new(1f, 1f); // How many in-world units a tile is

    private static Color BGColorMain = new(32, 18, 8);
    private static Color BGColorDark = new(16, 9, 4);


    // All these XML files are loaded into the single `sprites` dictionary on startup
    private static readonly string[] SpritePaths = new[]
    {
        @"Resources/Sprites/BountifulBits.xml",
        @"Resources/Sprites/BitBonanza.xml",
    };

    private static readonly (string, string)[] OtherSprites = new[]
    {
        ("Circle", @"Resources/Sprites/Circle.png"),
    };

    // These are loaded into `fonts` at startup. Each one makes a "font stack" -- order matters!
    private static readonly (string, string[])[] FontPaths = new[]
    {
        ("NotoMono", new[] {
            @"Resources/Fonts/NotoMono-Condensed-Medium.ttf",
            @"Resources/Fonts/NotoEmoji-Bold.ttf",
        }),
        ("DungeonFont", new[] {
            @"Resources/Fonts/DungeonFont.ttf",
            @"Resources/Fonts/NotoEmoji-Bold.ttf",
        }),
    };

    private static readonly string[] ExcludedFoods = new[]
    {
        "Crumbs.Red",
        "Crumbs.Orange",
        "Crumbs.Yellow",
        "Crumbs.Green",
        "Crumbs.Purple",
        "Seeds",
        "Wine.Empty",
        "Martini.Empty",
        "Whisky.Empty",
        "Ale.Empty",
        "Acorn",
        "Acorn.Alt",
    };

    private static readonly string[] StartupTileMap = new[]
    {
        "$$$$$$$$$$$$$$$$$", // $ = tree
        "$*             ,$", // , = little shrub
        "$$$~~~~~~~~~~~$$$", // * = big shrub
        "$,   '      '  *$", // ~ = fence
        "$ `          `  $", // ^ = yoshi cookie tile
        "$,  `       ''  $", // ` = rubble
        "$      '   '``  $", // ' = dust
        "$ ,         `'  $", // ! = skull & bones rubble
        "$  '     `      $", // # = castle wall
        "$ ``'       ' , $", // < = castle wall L
        "$ ``  `         $", // > = castle wall R
        "$       '       $", // . = normal floor
        "$           !'  $", // _ = diagonal floor tile
        "$   ! '         $",
        "$`           '  $", // see method in TileMap class
        "$,''`''` ` '`'',$",
        "$*,,''`'``'`',**$",
        "<###############>",
        "<...............>",
        "<..___________..>",
        "<...............>",
        "<.._________. ..>",
        "<...............>",
    };


    #endregion


    #region Internal fields

    private bool isInitialized = false;

    // --------------------------------------------
    // Rendering & assets

    // Since we have to wait for `LoadContent` to run before we have access to the graphics device, we can't initialize
    // most of these yet. Instead we just lie to the compiler and pretend they're non-null.

    // NB: Ensure we don't access this until LoadContent is done!
    private GraphicsDevice GraphicsDevice => Graphics.GraphicsDevice; // too lazy to always `Graphics.GraphicsDevice`...
    private SpriteBatch SpriteBatch = null!;
    private Texture2D Quad = null!;                         // 1x1 white pixel: tint + stretch = rectangle!

    private Dictionary<string, Sprite> Sprites = null!;     // all loaded sprites
    private Dictionary<string, FontSystem> Fonts = null!;   // all loaded fonts
    private readonly TileMap Tiles;                         // maps x,y to sprites
    private string[] AllFoodSprites;                        // a list of all food sprite names

    private static readonly int TileMapWidth = StartupTileMap.Select(line => line.Length).Max();
    private static readonly int TileMapHeight = StartupTileMap.Length;

    private RenderTarget2D MainPanel = null!;               // holds field and castle
    private RenderTarget2D InfoPanel = null!;               // holds the entity info boxes
    private RenderTarget2D TextPanel = null!;               // holds the student's print log

    private static Rectangle MainBounds;                    // Where the main panel appears on screen

    private Rectangle InfoBounds;                           // Where the entity info panel appears
    private Rectangle TextBounds;                           // Where the console panel appears
    private DepthStencilState StencilMask = null!;          // used to mask regions of the textPanel for scrolling

    private static readonly Color EnergyBarColor1 = new(25, 7, 181);
    private static readonly Color EnergyBarColor2 = new(16, 115, 235);

    // --------------------------------------------
    // Positioning

    private static readonly Vector2 TileScale = TileSizeScreen.ToVector2(); // Just a vec2 version of the constant up above

    // measured in in-world units, not pixels
    private static readonly Vector2 TileOffset = TileSizeWorld / 2;         // world-units to go from corner -> center
    private static readonly Vector2 OriginOffset = TilePosToWorldPos(8, 4); // to go from the *very* TL -> "origin" tile

    // Define pre-determined locations based on the tilemap
    private static readonly Vector2 FoodPosition = TilePosToWorldPos(13, 21);
    private static readonly HardcodedLine FrontLinePos = new HardcodedLine(new(3, 19), new(13, 19));
    private static readonly HardcodedLine RecoveryQueuePos = new HardcodedLine(new(11, 21), new(3, 21)); // R -> L
    private static readonly HardcodedLine BackupGoblinsPos = new HardcodedLine(new(3, 1), new(13, 1));

    // in-world positions that mark the edges of the inner playfield
    public static readonly Vector2 FieldTL;
    public static readonly Vector2 FieldTR;
    public static readonly Vector2 FieldBR;
    public static readonly Vector2 FieldBL;

    // --------------------------------------------
    // For game logic

    private int recoveryQueuePreCount;
    private Sprite? currentFood;

    // Using a seed lets me kinda fiddle with things to get the map looking how I want
    private static readonly Random SeededRNG = new(0x2024);

    // --------------------------------------------
    // Custom console

    private static readonly string NotifyIcon = "\U0001F514";

    private const int FontSize = 18;
    private const int LineSpacing = 6;

    private readonly LineWriter consoleWriter;      // captures console writes to display in-game
    private TextWriter? stdout;                     // keep a reference to the old one
    private TextWriter? stderr;                     // same for this one

    private Vector2 textOverflow;                   // how far the text has fallen out of bounds
    private float userXScroll = 0;                  // user horizontal scroll distance

    private bool _captureConsole = true;
    private int _consoleHeight = 280;

    // --------------------------------------------
    // Info boxes

    private const int InfoBoxSpacing = 4;
    private const int InfoBoxLineHeight = 20;

    #endregion


    #region Properties

    /// <summary>
    /// Controls whether or not the renderer should show <see cref="Console.Write"/> and <see cref="Console.WriteLine"/>
    /// output in its console window.
    /// </summary>
    public bool CaptureConsoleOutput
    {
        get => _captureConsole;
        set
        {
            _captureConsole = value;

            if (isInitialized)
            {
                ConfigurePanelSizes();
                if (_captureConsole) StartConsoleCapture();
                else StopConsoleCapture();
            }
        }
    }

    /// <summary>
    /// How tall, in pixels, the console panel should be.
    /// </summary>
    public int ConsolePanelSize
    {
        get => _captureConsole ? _consoleHeight : 0;
        set
        {
            if (value <= 0)
            {
                CaptureConsoleOutput = false; // save internal height, but disable capture
            }
            else
            {
                _consoleHeight = value;
                CaptureConsoleOutput = true;
            }
        }
    }

    #endregion


    #region Constructors

    // Constructor for actual renderer values (and things that depend on them)
    // ===========================================
    public CastleGameRenderer() : base(WindowWidth, WindowHeight)
    {
        Tiles = new TileMap(StartupTileMap, SeededRNG);   // non-static since it depends on graphics device stuff
        consoleWriter = new LineWriter();                   // Configured in Initialize
        AllFoodSprites = Array.Empty<string>();             // Replaced in LoadContent
    }


    // Static initializer for setting up constants
    // ===========================================
    static CastleGameRenderer()
    {
        Array.Sort(ExcludedFoods); // So we can BinarySearch in LoadContent

        // ----------------------------------------
        // Configure coordinates

        var mainP = new Point(PanelMargins, PanelMargins);
        var mainS = new Point(TileMapWidth, TileMapHeight) * TileSizeScreen;
        MainBounds = new Rectangle(mainP, mainS);

        // NB: These are shifted a little bit so the sprites bounce of the walls correctly
        FieldTL = TilePosToWorldPos(01, 03) + new Vector2(-0.1f, -0.1f);
        FieldTR = TilePosToWorldPos(15, 03) + new Vector2(+0.1f, -0.1f);
        FieldBL = TilePosToWorldPos(01, 16) + new Vector2(-0.1f, +0.1f);
        FieldBR = TilePosToWorldPos(15, 16) + new Vector2(+0.1f, +0.1f);
    }

    #endregion


    #region Math helpers

    private static Point WorldPosToScreenPos(Vector2 pos) => ((pos + OriginOffset) * TileScale).ToPoint();
    private static Point WorldPosToScreenPos(float x, float y) => WorldPosToScreenPos(new Vector2(x, y));

    private static Point TilePosToScreenPos(int x, int y) => new Point(x * TileSizeScreen.X, y * TileSizeScreen.Y);
    private static Point TilePosToScreenPos(Point pos) => TilePosToScreenPos(pos.X, pos.Y);

    private static Vector2 ScreenPosToWorldPos(Vector2 pos) => pos / TileScale - OriginOffset;
    private static Vector2 ScreenPosToWorldPos(Point pos) => ScreenPosToWorldPos(pos.ToVector2());
    private static Vector2 ScreenPosToWorldPos(int x, int y) => ScreenPosToWorldPos(new Vector2(x, y));

    private static Vector2 TilePosToWorldPos(Vector2 corner) => corner + TileOffset - OriginOffset;
    private static Vector2 TilePosToWorldPos(Point xyIdx) => TilePosToWorldPos(xyIdx.ToVector2());
    private static Vector2 TilePosToWorldPos(int x, int y) => TilePosToWorldPos(new Vector2(x, y));


    private static Rectangle WorldPosToDest(Vector2 worldPos)
    {
        // shift by tileOffset before scaling so it's centered
        Point screenPos = WorldPosToScreenPos(worldPos - TileOffset);
        return new Rectangle(screenPos, TileSizeScreen);
    }

    private static Rectangle WorldPosToDest(float x, float y) => WorldPosToDest(new Vector2(x, y));

    private static Rectangle TilePosToDest(Point xyIdx) => new Rectangle(TilePosToScreenPos(xyIdx), TileSizeScreen);
    private static Rectangle TilePosToDest(int x, int y) => new Rectangle(TilePosToScreenPos(x, y), TileSizeScreen);

    private static Rectangle ScreenPosToDest(Point screenPos) => new Rectangle(screenPos, TileSizeScreen);
    private static Rectangle ScreenPosToDest(int x, int y) => new Rectangle(new Point(x, y), TileSizeScreen);

    // https://stackoverflow.com/a/9493060/10549827
    private static Color HslToRgb(float h, float s, float l)
    {
        static float HueToRgbChannel(float p, float q, float t)
        {
            if (t < 0f) t += 1f;
            else if (t > 1f) t -= 1f;

            if (t < 1f / 6f) return p + (q - p) * 6f * t;
            else if (t < 1f / 2f) return q;
            else if (t < 2f / 3f) return p + (q - p) * (2f / 3f - t) * 6f;
            else return p;
        }

        float r;
        float g;
        float b;
        if (s == 0.0f)
        {
            // Achromatic
            r = g = b = l;
        }
        else
        {
            float q = (l < 0.5f)
                ? l * (1f + s)
                : l + s - (l * s);
            float p = 2f * l - q;

            r = HueToRgbChannel(p, q, h + 1f / 3f);
            g = HueToRgbChannel(p, q, h);
            b = HueToRgbChannel(p, q, h - 1f / 3f);
        }

        return new Color(r, g, b, 1.0f);
    }

    #endregion


    #region LoadContent


    protected override void LoadContent(CastleDefender userViz)
    {
        SpriteBatch = new SpriteBatch(GraphicsDevice);
        var loader = new AssetLoader(GraphicsDevice);

        Quad = new Texture2D(GraphicsDevice, 1, 1);
        Quad.SetData(new[] { Color.White });

        // ------------------------------
        // Load sprites

        Sprites = loader.LoadSpriteGroups(SpritePaths);
        foreach ((string name, string path) in OtherSprites)
        {
            Sprites[name] = loader.LoadSpriteFromImage(name, path);
        }

        // ------------------------------
        // Load fonts

        Fonts = new Dictionary<string, FontSystem>(FontPaths.Length);
        foreach ((string name, string[] paths) in FontPaths)
        {
            Fonts.Add(name, loader.LoadFont(paths));
        }

        // ------------------------------
        // Configure render targets

        MainPanel = MakeRenderTarget(GraphicsDevice, MainBounds.Size);
        InfoPanel = MakeRenderTarget(GraphicsDevice, InfoBounds.Size);
        TextPanel = MakeRenderTarget(GraphicsDevice, TextBounds.Size, true);

        DrawToRenderTarget(TextPanel, () =>
        {
            // Fill the stencil buffer with zeroes
            GraphicsDevice.Clear(ClearOptions.Target | ClearOptions.Stencil, Color.Transparent, 0, 0);

            // Now write into it with ones:
            SpriteBatch.Begin(depthStencilState: new DepthStencilState()
            {
                StencilEnable = true,
                StencilPass = StencilOperation.Replace,     // When passing, write directly to buffer.
                StencilFunction = CompareFunction.Always,   // Always pass.
                ReferenceStencil = 1,                       // Write a 1.
            });

            Rectangle region = new(Point.Zero, TextBounds.Size);
            region.Inflate(-PanelPadding, -PanelPadding);

            SpriteBatch.Draw(Quad, region, Color.White);
            SpriteBatch.End();
        });

        // Then configure a new depth/stencil state that fails whenever the pixel under it isn't white.
        StencilMask = new DepthStencilState()
        {
            StencilEnable = true,
            StencilPass = StencilOperation.Keep,            // Don't modify the stencil buffer, even if you pass
            StencilFunction = CompareFunction.Equal,        // Only pass the stencil test if value is equal to...
            ReferenceStencil = 1,                           // the 1 we wrote before.
        };

        // ------------------------------
        // Other content-dependent setup

        Tiles.LoadSprites(Sprites);

        // Iterating over every single loaded sprite is *so* inefficient... could have just kept them in their tree
        // structure to begin with... oh well. Don't have time to rewrite sprite loading AGAIN.
        AllFoodSprites = Sprites
            .Where(pair => pair.Key.StartsWith("Food/"))
            .Select(pair => pair.Value.Name)
            .Where(name => Array.BinarySearch(ExcludedFoods, name) < 0)
            .Select(name => "Food/" + name)
            .ToArray();

        // Now that sprites are loaded, we can pick a food :)
        PickNewFood();
    }


    private static RenderTarget2D MakeRenderTarget(GraphicsDevice graphicsDevice, Point size, bool stencil = false)
    {
        return new RenderTarget2D(
            graphicsDevice,
            size.X,
            size.Y,
            mipMap: false,
            SurfaceFormat.Color,
            stencil ? DepthFormat.Depth24Stencil8 : DepthFormat.Depth24,
            preferredMultiSampleCount: 4,
            RenderTargetUsage.PreserveContents
        );
    }

    #endregion


    #region Initialize


    protected override void Initialize(CastleDefender userViz)
    {
        ConfigurePanelSizes();
        if (CaptureConsoleOutput) StartConsoleCapture();
        else StopConsoleCapture();

        UserInput.KeyPressed += HandleKeyPress;
        UserInput.MouseScrolled += HandleScroll;

        isInitialized = true;

        // if (userViz.RecoveryQueue is not null && userViz.RecoveryQueue is not IEnumerable<CombatEntity>)
        // {
        //     Console.WriteLine($"{WarningIcon} Your Queue<T> does not not IEnumerable yet.");
        // }
    }


    private void StartConsoleCapture(bool notify = true)
    {
        if (notify)
            Console.WriteLine($"{NotifyIcon} Further console output will appear in the game window.");

        try
        {
            Console.Out.Flush();
            Console.Error.Flush();
            // Using the same destination for both redirects shouldn't cause a problem as long as things stay
            // single-threaded... (and even then the odds of them using stderr are slim).
            stdout = Console.Out;
            stderr = Console.Error;
            Console.SetOut(consoleWriter);
            Console.SetError(consoleWriter);
        }
        catch (Exception ex) when (ex is IOException or SecurityException)
        {
            // If we fail to redirect output, simply disable our custom console altogether.
            _captureConsole = false; // inhibit setter
            StopConsoleCapture(false);
            ConfigurePanelSizes();
        }
    }


    private void StopConsoleCapture(bool notify = true)
    {
        if (notify)
            Console.WriteLine($"{NotifyIcon} Further console output will appear in your terminal.");

        if (stdout != null)
        {
            Console.Out.Flush();
            Console.SetOut(stdout);
            stdout = null;
        }

        if (stderr != null)
        {
            Console.Error.Flush();
            Console.SetError(stderr);
            stderr = null;
        }
    }

    protected override void Cleanup()
    {
        if (CaptureConsoleOutput) StopConsoleCapture(false);
    }


    #endregion


    #region PanelSizes


    private void ConfigurePanelSizes()
    {
        int margin = PanelMargins;
        int tSize = _consoleHeight; // Use internal one since that one is clamped > 0, preventing a Texture2D crash
        int w = WindowWidth;
        int h = WindowHeight;

        // P = pos, S = size. Basically, main panel is 17x23 tiles. Text panel is `t` pixels tall. 15 pixels between
        // each. The info/text panels take up however much space is leftover.

        var textS = new Point(w - MainBounds.Width - margin * 3, tSize);
        var textP = new Point(MainBounds.Right + margin, h - margin - tSize);
        TextBounds = new Rectangle(textP, textS);

        var infoP = new Point(MainBounds.Right + margin, MainBounds.Y);
        var infoS = new Point(w - infoP.X - margin, h - tSize - margin * 3);
        InfoBounds = new Rectangle(infoP, infoS);

        if (!CaptureConsoleOutput)
        {
            TextBounds.Y = h * 2;               // Move off the bottom of the screen
            InfoBounds.Height = h - margin * 2; // Use full height
        }
    }

    #endregion


    #region Update


    protected override void PreUpdate(CastleDefender userViz, GameTime gameTime, bool willDoUserUpdate)
    {
        if (willDoUserUpdate)
            recoveryQueuePreCount = CountRecoveryWizards(userViz);

        // Determine text overflow
        if (CaptureConsoleOutput)
        {
            var innerBounds = new Rectangle(Point.Zero, TextBounds.Size);
            innerBounds.Inflate(-PanelPadding, -PanelPadding);
            var farCorner = new Vector2(innerBounds.Right, innerBounds.Bottom);

            var text = consoleWriter.Text;
            var font = Fonts["NotoMono"].GetFont(FontSize);
            var size = font.MeasureString(text, lineSpacing: LineSpacing);

            Vector2 textCorner = new Vector2(PanelPadding) + size;
            Vector2 overflow = textCorner - farCorner;
            textOverflow = Vector2.Max(Vector2.Zero, overflow);
        }
    }


    protected override void PostUpdate(CastleDefender userViz, GameTime gameTime, bool didUserUpdate)
    {
        if (didUserUpdate)
        {
            // If the user has fewer wizards in their queue after the frame than they did when the frame started, it
            // probably means one of them is done eating, so we pick a new food.
            if (CountRecoveryWizards(userViz) < recoveryQueuePreCount)
                PickNewFood();

            // Snap the non-goblin entities into their positions, so the student doesn't have to worry about positioning
            // them into the right places (use a pattern-match/cast just in case they haven't implemented IEnumerable
            // yet, so they don't get a red squiggly halfway down this massive file)
            if (userViz.WizardSquad is IEnumerable<Wizard> ws) FrontLinePos.SetPositions(ws);
            if (userViz.RecoveryQueue is IEnumerable<Wizard> rq) RecoveryQueuePos.SetPositions(rq, 1.0f);
            if (userViz.BackupGoblins is IEnumerable<Goblin> bg) BackupGoblinsPos.SetPositions(bg, 1.0f);
        }
    }


    private void PickNewFood()
    {
        string name = AllFoodSprites[Program.RNG.Next(AllFoodSprites.Length)];
        currentFood = Sprites[name];
    }


    private static int CountRecoveryWizards(CastleDefender userViz)
    {
        // Attempt to access the 'Count' or 'count' property of the user's Queue, but don't crash or show an error if it
        // doesn't exist, or if the queue is null.
        var queue = userViz.RecoveryQueue;
        if (queue is null)
            return 0;

        var qType = queue.GetType();
        var upper = qType.GetProperty("Count", typeof(int));
        var lower = qType.GetProperty("count", typeof(int));
        return (upper?.GetValue(queue) as int?) ?? (lower?.GetValue(queue) as int?) ?? 0;
    }


    #endregion


    #region Drawing


    protected override void Draw(CastleDefender userViz, GameTime gameTime, bool didUserUpdate)
    {
        DrawToRenderTarget(MainPanel, () => DrawMainPanel(userViz));
        DrawToRenderTarget(InfoPanel, () => DrawInfoPanel(userViz));
        DrawToRenderTarget(TextPanel, () => DrawTextPanel());

        // this doesn't work if it comes before the render targets, for some reason. Even tho DrawToTargets sets its
        // target back to the main framebuffer... hrm. oh well.
        GraphicsDevice.Clear(BGColorMain);

        SpriteBatch.Begin();
        SpriteBatch.Draw(MainPanel, MainBounds, Color.White);
        SpriteBatch.Draw(InfoPanel, InfoBounds, Color.White);
        SpriteBatch.Draw(TextPanel, TextBounds, Color.White);
        SpriteBatch.End();
    }

    private void DrawToRenderTarget(RenderTarget2D target, Action drawFn)
    {
        GraphicsDevice.SetRenderTarget(target);
        drawFn();
        GraphicsDevice.SetRenderTarget(null);
    }


    #region Main panel

    private void DrawMainPanel(CastleDefender userViz)
    {
        // Notes:
        // - SamplerState.PointClamp: gives nearest-neighbour-like interpolation.
        // - BlendState.NonPremultiplied: we're using PNGs directly, so non-premultiplied alpha is more intuitive.

        GraphicsDevice.Clear(Color.Transparent);

        DrawBackgroundTiles();
        DrawEntities(userViz);
    }


    private void DrawBackgroundTiles()
    {
        SpriteBatch.Begin(samplerState: SamplerState.PointClamp, blendState: BlendState.NonPremultiplied);

        for (int y = 0; y < Tiles.Height; y++)
        {
            for (int x = 0; x < Tiles.Width; x++)
            {
                Sprite? tile = Tiles[x, y];
                if (tile != null)
                {
                    SpriteBatch.Draw(tile, TilePosToDest(x, y), Color.White);
                }
            }
        }

        // Draw the food
        if (currentFood != null)
            SpriteBatch.Draw(currentFood, WorldPosToDest(FoodPosition), Color.White);

        SpriteBatch.End();
    }

    // Again, try to convert to an iterable collection, but don't crash or fail or throw any red squigglies if their
    // things aren't quite right.
    private static IEnumerable<CombatEntity> CoalesceEnumerable(object? maybeEnumerable)
    {
        return maybeEnumerable as IEnumerable<CombatEntity> ?? Enumerable.Empty<CombatEntity>();
    }


    private void DrawEntities(CastleDefender userViz)
    {

        var allEntities = CoalesceEnumerable(userViz.WizardSquad)
            .Concat(CoalesceEnumerable(userViz.RecoveryQueue))
            .Concat(CoalesceEnumerable(userViz.GoblinSquad))
            .Concat(CoalesceEnumerable(userViz.BackupGoblins))
            .Concat(CoalesceEnumerable(userViz.Spells));
        var activeWizard = userViz.ActiveWizard?.Item;

        SpriteBatch.Begin(samplerState: SamplerState.PointClamp, blendState: BlendState.NonPremultiplied);

        foreach (CombatEntity entity in allEntities)
        {
            bool isActive = ReferenceEquals(activeWizard, entity);
            DrawEntitySprite(entity, isActive);

            if (entity is Wizard wiz)
                DrawWizardHealthBar(wiz);
        }

        SpriteBatch.End();

        // For text
        SpriteBatch.Begin();

        foreach (CombatEntity entity in allEntities)
        {
            if (entity is Wizard or Goblin)
                DrawEntityID(entity);
        }

        SpriteBatch.End();
    }


    private void DrawEntitySprite(CombatEntity entity, bool highlight = false)
    {
        Rectangle spriteDest = WorldPosToDest(entity.Position);
        DrawEntitySprite(entity, spriteDest, highlight);
    }

    private void DrawEntitySprite(CombatEntity entity, Rectangle destination, bool highlight = false)
    {
        Color tint = entity.RendererTintColor;
        Sprite sprite = Sprites[entity.SpriteName];

        if (highlight)
        {
            Rectangle hlDest = destination;
            hlDest.Inflate(TileSizeScreen.X / 8, TileSizeScreen.Y / 8);
            SpriteBatch.Draw(Quad, hlDest, new Color(1f, 1f, 0.5f, 0.25f));
        }

        SpriteBatch.Draw(sprite, destination, tint);
    }


    private void DrawWizardHealthBar(Wizard wizard)
    {
        // Go down from the wizard
        var wPos = wizard.Position + new Vector2(0, 0.8f);
        var sPos = WorldPosToScreenPos(wPos);

        int baseW = TileSizeScreen.X * 9 / 10;
        int baseH = TileSizeScreen.Y / 4;
        sPos.X -= baseW / 2;
        sPos.Y -= baseH / 2;

        float fillFactor = wizard.Energy / (float)wizard.MaxEnergy;
        int fillW = (int)(baseW * fillFactor);

        Rectangle baseDest = new(sPos, new Point(baseW, baseH));
        Rectangle fillDest = new(sPos, new Point(fillW, baseH));
        SpriteBatch.Draw(Quad, baseDest, EnergyBarColor1);
        SpriteBatch.Draw(Quad, fillDest, EnergyBarColor2);
    }


    private void DrawEntityID(CombatEntity entity)
    {
        var ePos = entity.Position + new Vector2(0, -0.8f);
        var sPos = WorldPosToScreenPos(ePos).ToVector2();

        var text = entity.ID.ToString();
        var font = Fonts["NotoMono"].GetFont(FontSize);

        var textSize = font.MeasureString(text);
        sPos -= textSize / 2;

        font.DrawText(SpriteBatch, text, sPos, Color.White);
    }

    #endregion


    #region Info panel

    private void DrawInfoPanel(CastleDefender userViz)
    {
        int bh = InfoBoxLineHeight * 2 + InfoBoxSpacing * 3;    // box height
        int bw = (InfoBounds.Width - InfoBoxSpacing * 2) / 3;   // box width

        GraphicsDevice.Clear(BGColorMain);

        // Draw the main wizards first

        var frontLine = CoalesceEnumerable(userViz.WizardSquad);

        Rectangle infoBox = new(0, 0, bw, bh);
        foreach (var entity in frontLine)
        {
            if (entity is Wizard wizard)
            {
                DrawWizardInfo(wizard, infoBox);
                NextBox(ref infoBox);
            }
        }

        // Then draw the recovery queue halfway down
        infoBox.Location = new(0, InfoBounds.Height / 2);

        var queue = CoalesceEnumerable(userViz.RecoveryQueue);
        foreach (var entity in queue)
        {
            if (entity is Wizard wizard)
            {
                DrawWizardInfo(wizard, infoBox);
                NextBox(ref infoBox);
            }
        }
    }


    private void NextBox(ref Rectangle infoBox)
    {
        infoBox.X += infoBox.Width + InfoBoxSpacing;
        if (infoBox.Right > InfoBounds.Width)
        {
            infoBox.X = 0;
            infoBox.Y += infoBox.Height + InfoBoxSpacing;
        }
    }


    private void DrawWizardInfo(Wizard wizard, Rectangle bounds)
    {
        var font = Fonts["NotoMono"].GetFont(InfoBoxLineHeight);

        var (x, y) = bounds.Location;
        int lh = InfoBoxLineHeight;
        int sp = InfoBoxSpacing;

        int innerMarginL = x + sp;
        int l1MarginTop = y + sp;
        int l2MarginTop = l1MarginTop + lh + sp;

        SpriteBatch.Begin(samplerState: SamplerState.PointClamp);

        // Draw the box background
        SpriteBatch.Draw(Quad, bounds, BGColorDark);

        // Draw the "portrait"
        Rectangle spriteDest = new(innerMarginL, l1MarginTop, lh, lh);
        DrawEntitySprite(wizard, spriteDest);

        // Draw the name a little to the side (double spacing)
        var text = $"#{wizard.ID} - {wizard.Name}";
        var textPos = new Vector2(innerMarginL + lh + sp * 2, l1MarginTop);
        SpriteBatch.DrawString(font, text, textPos, Color.White);

        // ------------------------------------------------------------------

        // Draw the spell icon
        Sprite spell = Sprites[Spell.GetElementSprite(wizard.SpellType) ?? CombatEntity.DefaultSprite];
        Rectangle spellDest = new(innerMarginL, l2MarginTop, lh, lh);
        SpriteBatch.Draw(spell, spellDest, Color.White);

        text = $"Level {wizard.SpellLevel}: {wizard.Energy}/{wizard.MaxEnergy} energy";
        textPos = new Vector2(innerMarginL + lh + sp * 2, l2MarginTop);
        SpriteBatch.DrawString(font, text, textPos, Color.White);

        SpriteBatch.End();
    }


    #endregion


    #region Text panel

    private void DrawTextPanel()
    {
        if (!CaptureConsoleOutput)
            return;

        var text = consoleWriter.Text;
        var font = Fonts["NotoMono"].GetFont(FontSize);

        var pos = new Vector2(PanelPadding);

        pos.Y -= textOverflow.Y;
        pos.X -= userXScroll;

        GraphicsDevice.Clear(ClearOptions.Target, BGColorDark, 0, 0); // Only clear the color buffer, not stencil buffer
        SpriteBatch.Begin(depthStencilState: StencilMask);
        font.DrawText(SpriteBatch, text, pos, Color.White, lineSpacing: LineSpacing);
        SpriteBatch.End();
    }

    #endregion


    #endregion


    #region Input

    private void HandleKeyPress(Key key)
    {
        switch (key)
        {
            case Key.Escape:
                Exit();
                break;

            case Key.Space:
                if (IsPaused)
                    Console.WriteLine($"{NotifyIcon} Resuming playback.");
                else
                    Console.WriteLine($"{NotifyIcon} Pausing playback.");
                IsPlaying = !IsPlaying;
                break;

            case Key.Right:
                if (IsPaused)
                {
                    Console.WriteLine($"{NotifyIcon} processing frame {CurrentFrame + 1}...");
                    StepForward();
                }
                break;
        }
    }


    private void HandleScroll(int y, int x)
    {
        userXScroll += x / 10f; // tone down scroll speed a bit
        userXScroll = Math.Clamp(userXScroll, 0, textOverflow.X);
    }


    #endregion


    #region HardcodedLine


    /// <summary>
    /// Used to "snap" entities into place in a line in a hard-coded location in the game world.
    /// </summary>
    private class HardcodedLine
    {
        public Vector2 Start { get; }
        public Vector2 End { get; }
        public float Length { get; }

        private readonly List<CombatEntity?> buffer;

        public HardcodedLine(Point startTile, Point endTile)
        {
            buffer = new();
            Start = TilePosToWorldPos(startTile);
            End = TilePosToWorldPos(endTile);
            Length = Vector2.Distance(End, Start);
        }

        /// <summary>
        /// Snaps the positions of the provided entities along this line, with equal space between its start and end.
        /// </summary>
        public void SetPositions(IEnumerable<CombatEntity?> entities)
        {
            // Need to know how many there are, so we need to `.collect()` them all. We keep the `buffer` around so we
            // don't have to always allocate new arrays (we can keep our capacity on subsequent calls).
            buffer.Clear();
            buffer.AddRange(entities);
            float amount = 1.0f / (buffer.Count - 1) * Length;
            SetPositions(buffer, amount); // their enumerable is consumed, need to use buffer
        }

        /// <summary>
        /// Snaps the positions of the provided entities along this line at a set distance apart.
        /// </summary>
        public void SetPositions(IEnumerable<CombatEntity?> entities, float distance)
        {
            Vector2 add = Vector2.Normalize(End - Start) * distance;
            Vector2 pos = Start;
            foreach (CombatEntity? entity in entities)
            {
                if (entity is not null)
                {
                    entity.Position = pos;
                    pos += add;
                }
            }
        }
    }


    #endregion


    #region TileMap


    private class TileMap
    {
        private Sprite[]? sprites;              // actually loaded sprite references
        private readonly string[] spriteNames;  // the unique sprite names found in tilemap, before converting to sprites
        private readonly int[,] lookupTable;    // (x, y) of indices into the above sprites array

        /// <summary>
        /// Whether or not this TileMap has actually loaded its sprites yet. If <c>false</c>, it has determined sprite
        /// <i>names</i>, but not actually collected <see cref="Sprite"/> references.
        /// </summary>
        [MemberNotNullWhen(true, nameof(sprites))]
        public bool IsInitialized { get; private set; }

        /// <summary>
        /// Gets a sprite from the tile-map at a particular (x, y) index.
        /// </summary>
        /// <param name="x">The horizontal offset, starting from the left..</param>
        /// <param name="y">The vertical offset, starting from the top.</param>
        /// <returns>
        /// Either a reference to a loaded sprite, or <c>null</c> if the given tile has no sprite associated with it (or
        /// <c>null</c> if sprites haven't <see cref="LoadSprites">been loaded yet</see>).
        /// </returns>
        public Sprite? this[int x, int y] => (IsInitialized && lookupTable[x, y] is int i and (> -1)) ? sprites[i] : null;

        /// <summary>
        /// How many tiles wide this tilemap is. Passing a non-rectangular char-map will inherit the width of the longest
        /// line, filling the rest with blanks.
        /// </summary>
        public int Width { get; }

        /// <summary>
        /// How many tiles tall this tilemap is.
        /// </summary>
        public int Height { get; }

        // --------------------------------------------

        public TileMap(string[] lines) : this(lines, Program.RNG)
        { }


        public TileMap(string[] lines, Random random)
        {
            if (lines.Length == 0)
                throw new ArgumentException("Need at least one line of tiles.", nameof(lines));

            Width = lines.Select(line => line.Length).Max();
            Height = lines.Length;

            lookupTable = new int[Width, Height];
            var sprites = new List<string>(); // There's only like 15 supported tiles, so an O(n) lookup should be quick

            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    if (x >= lines[y].Length || GetTileSpriteName(lines[y][x], random) is not string spriteName)
                    {
                        lookupTable[x, y] = -1;
                    }
                    else if (sprites.IndexOf(spriteName) is int existingIndex and >= 0)
                    {
                        lookupTable[x, y] = existingIndex;
                    }
                    else
                    {
                        lookupTable[x, y] = sprites.Count; // currently 3 sprites -> new one will be at index 3
                        sprites.Add(spriteName);
                    }
                }
            }

            spriteNames = sprites.ToArray();
            IsInitialized = false;
            // MarkerTiles = markers.ToArray();
        }

        // --------------------------------------------

        /// <summary>
        /// Populates this tilemap's collection of sprites.
        /// </summary>
        /// <param name="loadedSprites">The dictionary returned by <see cref="AssetLoader.LoadSpriteGroups"/>.</param>
        public void LoadSprites(Dictionary<string, Sprite> loadedSprites)
        {
            sprites = spriteNames.Select(name => loadedSprites[name]).ToArray();
            IsInitialized = true;
        }

        /// <inheritdoc cref="GetTileSpriteName(char, Random)"/>
        public static string? GetTileSpriteName(char symbol) => GetTileSpriteName(symbol, Program.RNG);

        /// <summary>
        /// Maps a tile's <see cref="char"/> representation into a sprite name.
        /// </summary>
        /// <param name="symbol">The <see cref="char"/> of the tile.</param>
        /// <param name="random">A pre-seeded <see cref="Random"/> to use for adding variation to tile sprites.</param>
        /// <returns>The string name of the sprite for that tile.</returns>
        public static string? GetTileSpriteName(char symbol, Random random) => symbol switch
        {
            ' ' => null,
            '~' => "Decor/Fences/Thick.LightBrown",
            '^' => "Buildings/Floors/Gray/YoshiCookie",     // marker tiles
            '.' => "Buildings/Floors/Gray/WoodPanels",      // main floor
            '_' => "Buildings/Floors/Gray/Diagonal",        // queue floor
            '#' => "Buildings/CastleWalls/Bricks/Full",
            '<' => "Buildings/CastleWalls/Bricks/Full.L",
            '>' => "Buildings/CastleWalls/Bricks/Full.R",
            '!' => "Debris/SkullBones.Gray",
            '$' => random.NextDouble() < 0.80 ? "Trees/Green/Pine.Pair" : "Trees/Green/Pine.Pair.Seeds",
            ',' => random.NextDouble() < 0.50 ? "Grass/Green/Weeds.Stage1" : "Grass/Green/Weeds.Stage1.Alt",
            '*' => random.NextDouble() < 0.70 ? "Shrubs/Green/Grown" : "Shrubs/Green/Grown.Pair",
            '`' => random.NextDouble() < 0.50 ? "Debris/Rubble.Brown" : "Debris/Rubble.Brown.Alt",
            '\'' => random.NextDouble() switch // small debris
            {
                >= 0.00 and < 0.75 => "Debris/Dust.Brown",
                >= 0.75 and < 0.90 => "Debris/Pebbles.Brown",
                >= 0.90 and < 1.00 => "Debris/Pebbles.Brown.Alt",
                _ => throw new Exception("Unreachable Exception: NextDouble() always returns [0, 1)."),
            },
            _ => "Symbols/QuestionMark",
        };
    }

    #endregion
}
