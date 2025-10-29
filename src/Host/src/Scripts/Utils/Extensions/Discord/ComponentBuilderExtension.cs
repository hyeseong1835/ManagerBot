using System;
using System.Collections.Generic;

using Discord;

namespace ManagerBot.Utils.Extension.Discord;

public static class ComponentBuilderExtension
{
    public static ComponentBuilder WithSelectMenu<TElement>(this ComponentBuilder builder, string customId,
        IEnumerable<TElement> options, Func<int, TElement, string>? label = null, Func<int, TElement, string>? value = null, string emptyLabel = "-", string emptyValue = "NULL",
        string? placeholder = null, int maxValues = 1, int minValues = 1, bool disabled = false,
        ComponentType type = ComponentType.SelectMenu, List<ChannelType>? channelTypes = null, List<SelectMenuDefaultValue>? defaultValues = null)
    {
        List<SelectMenuOptionBuilder> menuOptions = new();

        int index = -1;
        foreach (var option in options)
        {
            index++;

            if (option == null) continue;

            menuOptions.Add(
                new SelectMenuOptionBuilder(
                    label == null ?
                        option.ToString()
                        : label(index, option),
                    value == null ?
                        option.ToString()
                        : value(index, option)
                )
            );
        }
        if (index == -1)
        {
            menuOptions.Add(new SelectMenuOptionBuilder(emptyLabel, emptyValue));
            disabled = true;
        }
        builder.WithSelectMenu(
            new SelectMenuBuilder(
                customId, menuOptions, placeholder,
                maxValues, minValues, disabled,
                type, channelTypes, defaultValues
            )
        );
        return builder;
    }
}
