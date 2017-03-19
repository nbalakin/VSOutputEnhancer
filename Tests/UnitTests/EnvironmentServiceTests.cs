﻿using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Windows.Media;
using FluentAssertions;
using Microsoft.VisualStudio.Text.Classification.Fakes;
using Microsoft.VisualStudio.Text.Formatting;
using Xunit;

namespace Balakin.VSOutputEnhancer.Tests.UnitTests
{
    [ExcludeFromCodeCoverage]
    public class EnvironmentServiceTests
    {
        [Theory]
        [MemberData(nameof(CreateColorsTestData))]
        public void GetTheme(Color? foreground, Color? background, Theme? expectedTheme)
        {
            var textProperties = Utils.CreateTextFormattingRunProperties(foreground, background);
            TestTheme(textProperties, expectedTheme);
        }

        [Theory]
        [MemberData(nameof(CreateBrushesTestData))]
        public void GetTheme(Brush foreground, Brush background, Theme? expectedTheme)
        {
            var textProperties = Utils.CreateTextFormattingRunProperties(foreground, background);
            TestTheme(textProperties, expectedTheme);
        }

        public static IEnumerable<object[]> CreateColorsTestData()
        {
            var defaultTheme = Theme.Light;

            yield return new Object[] { null, null, defaultTheme };

            yield return new Object[] { Colors.Black, null, Theme.Light };
            yield return new Object[] { null, Colors.White, Theme.Light };
            yield return new Object[] { Colors.Black, Colors.White, Theme.Light };

            yield return new Object[] { Colors.White, null, Theme.Dark };
            yield return new Object[] { null, Colors.Black, Theme.Dark };
            yield return new Object[] { Colors.White, Colors.Black, Theme.Dark };
        }

        public static IEnumerable<object[]> CreateBrushesTestData()
        {
            var defaultTheme = Theme.Light;

            yield return new Object[] { null, new RadialGradientBrush(), defaultTheme };
            yield return new Object[] { new RadialGradientBrush(), null, defaultTheme };
            yield return new Object[] { new RadialGradientBrush(), new RadialGradientBrush(), defaultTheme };

            yield return new Object[] { new SolidColorBrush(Colors.Black), new RadialGradientBrush(), Theme.Light };
            yield return new Object[] { new RadialGradientBrush(), new SolidColorBrush(Colors.White), Theme.Light };

            yield return new Object[] { new SolidColorBrush(Colors.White), new RadialGradientBrush(), Theme.Dark };
            yield return new Object[] { new RadialGradientBrush(), new SolidColorBrush(Colors.Black), Theme.Dark };
        }

        private void TestTheme(TextFormattingRunProperties textProperties, Theme? expectedTheme)
        {
            var classificationFormatMapService = new StubIClassificationFormatMapService();
            classificationFormatMapService.GetClassificationFormatMapString = category =>
            {
                return new StubIClassificationFormatMap
                {
                    DefaultTextPropertiesGet = () => textProperties
                };
            };

            var environmentService = Utils.CreateEnvironmentService(classificationFormatMapService);
            var actualTheme = environmentService.GetTheme();
            actualTheme.Should().Be(expectedTheme);
        }
    }
}