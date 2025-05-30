// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Diagnostics;
using System.Linq;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Localisation;
using osu.Game.Beatmaps;
using osu.Game.Beatmaps.Drawables;
using osu.Game.Graphics;
using osu.Game.Graphics.Carousel;
using osu.Game.Graphics.Sprites;
using osu.Game.Overlays;
using osuTK;

namespace osu.Game.Screens.SelectV2
{
    public partial class PanelBeatmapSet : Panel
    {
        public const float HEIGHT = CarouselItem.DEFAULT_HEIGHT * 1.7f;

        private PanelSetBackground background = null!;

        private OsuSpriteText titleText = null!;
        private OsuSpriteText artistText = null!;
        private Drawable chevronIcon = null!;
        private PanelUpdateBeatmapButton updateButton = null!;
        private BeatmapSetOnlineStatusPill statusPill = null!;
        private DifficultySpectrumDisplay difficultiesDisplay = null!;

        [Resolved]
        private OverlayColourProvider colourProvider { get; set; } = null!;

        [Resolved]
        private BeatmapManager beatmaps { get; set; } = null!;

        public PanelBeatmapSet()
        {
            PanelXOffset = 20f;
        }

        [BackgroundDependencyLoader]
        private void load()
        {
            Height = HEIGHT;

            Icon = chevronIcon = new Container
            {
                Size = new Vector2(0, 22),
                Child = new SpriteIcon
                {
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    Icon = FontAwesome.Solid.ChevronRight,
                    Size = new Vector2(8),
                    X = 1f,
                    Colour = colourProvider.Background5,
                },
            };

            Background = background = new PanelSetBackground
            {
                RelativeSizeAxes = Axes.Both,
            };

            Content.Children = new[]
            {
                new FillFlowContainer
                {
                    AutoSizeAxes = Axes.Both,
                    Direction = FillDirection.Vertical,
                    Padding = new MarginPadding { Top = 7.5f, Left = 15, Bottom = 5 },
                    Children = new Drawable[]
                    {
                        titleText = new OsuSpriteText
                        {
                            Font = OsuFont.Style.Heading1.With(typeface: Typeface.TorusAlternate),
                        },
                        artistText = new OsuSpriteText
                        {
                            Font = OsuFont.Style.Body.With(weight: FontWeight.SemiBold),
                        },
                        new FillFlowContainer
                        {
                            Direction = FillDirection.Horizontal,
                            AutoSizeAxes = Axes.Both,
                            Margin = new MarginPadding { Top = 4f },
                            Children = new Drawable[]
                            {
                                updateButton = new PanelUpdateBeatmapButton
                                {
                                    Anchor = Anchor.CentreLeft,
                                    Origin = Anchor.CentreLeft,
                                    Margin = new MarginPadding { Right = 5f, Top = -2f },
                                },
                                statusPill = new BeatmapSetOnlineStatusPill
                                {
                                    Origin = Anchor.CentreLeft,
                                    Anchor = Anchor.CentreLeft,
                                    TextSize = OsuFont.Style.Caption2.Size,
                                    Margin = new MarginPadding { Right = 5f },
                                    Animated = false,
                                },
                                difficultiesDisplay = new DifficultySpectrumDisplay
                                {
                                    Anchor = Anchor.CentreLeft,
                                    Origin = Anchor.CentreLeft,
                                },
                            },
                        }
                    }
                }
            };
        }

        protected override void LoadComplete()
        {
            base.LoadComplete();

            Expanded.BindValueChanged(_ => onExpanded(), true);
            KeyboardSelected.BindValueChanged(k => KeyboardSelected.Value = k.NewValue, true);
        }

        private void onExpanded()
        {
            if (Expanded.Value)
            {
                chevronIcon.ResizeWidthTo(18, 600, Easing.OutElasticQuarter);
                chevronIcon.FadeTo(1f, DURATION, Easing.OutQuint);
            }
            else
            {
                chevronIcon.ResizeWidthTo(0f, DURATION, Easing.OutQuint);
                chevronIcon.FadeTo(0f, DURATION, Easing.OutQuint);
            }
        }

        protected override void PrepareForUse()
        {
            base.PrepareForUse();

            Debug.Assert(Item != null);

            var beatmapSet = (BeatmapSetInfo)Item.Model;

            // Choice of background image matches BSS implementation (always uses the lowest `beatmap_id` from the set).
            background.Beatmap = beatmaps.GetWorkingBeatmap(beatmapSet.Beatmaps.MinBy(b => b.OnlineID));

            titleText.Text = new RomanisableString(beatmapSet.Metadata.TitleUnicode, beatmapSet.Metadata.Title);
            artistText.Text = new RomanisableString(beatmapSet.Metadata.ArtistUnicode, beatmapSet.Metadata.Artist);
            updateButton.BeatmapSet = beatmapSet;
            statusPill.Status = beatmapSet.Status;
            difficultiesDisplay.BeatmapSet = beatmapSet;
        }

        protected override void FreeAfterUse()
        {
            base.FreeAfterUse();

            background.Beatmap = null;
            updateButton.BeatmapSet = null;
            difficultiesDisplay.BeatmapSet = null;
        }
    }
}
