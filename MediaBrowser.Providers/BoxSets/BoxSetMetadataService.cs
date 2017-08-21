﻿using MediaBrowser.Controller.Configuration;
using MediaBrowser.Controller.Entities;
using MediaBrowser.Controller.Entities.Movies;
using MediaBrowser.Controller.Library;
using MediaBrowser.Controller.Providers;
using MediaBrowser.Model.Entities;
using MediaBrowser.Model.Logging;
using MediaBrowser.Providers.Manager;
using System.Linq;
using MediaBrowser.Model.IO;
using MediaBrowser.Model.Extensions;

namespace MediaBrowser.Providers.BoxSets
{
    public class BoxSetMetadataService : MetadataService<BoxSet, BoxSetInfo>
    {
        protected override ItemUpdateType BeforeSave(BoxSet item, bool isFullRefresh, ItemUpdateType currentUpdateType)
        {
            var updateType = base.BeforeSave(item, isFullRefresh, currentUpdateType);

            if (isFullRefresh || currentUpdateType > ItemUpdateType.None)
            {
                if (!item.LockedFields.Contains(MetadataFields.OfficialRating))
                {
                    if (item.UpdateRatingToContent())
                    {
                        updateType = updateType | ItemUpdateType.MetadataEdit;
                    }
                }
            }

            return updateType;
        }

        protected override void MergeData(MetadataResult<BoxSet> source, MetadataResult<BoxSet> target, MetadataFields[] lockedFields, bool replaceData, bool mergeMetadataSettings)
        {
            ProviderUtils.MergeBaseItemData(source, target, lockedFields, replaceData, mergeMetadataSettings);

            var sourceItem = source.Item;
            var targetItem = target.Item;

            if (mergeMetadataSettings)
            {
                // Retain shortcut children
                var linkedChildren = sourceItem.LinkedChildren.ToList();
                linkedChildren.AddRange(sourceItem.LinkedChildren.Where(i => i.Type == LinkedChildType.Shortcut));

                targetItem.LinkedChildren = linkedChildren.ToArray(linkedChildren.Count);
                targetItem.Shares = sourceItem.Shares;
            }
        }

        public BoxSetMetadataService(IServerConfigurationManager serverConfigurationManager, ILogger logger, IProviderManager providerManager, IFileSystem fileSystem, IUserDataManager userDataManager, ILibraryManager libraryManager) : base(serverConfigurationManager, logger, providerManager, fileSystem, userDataManager, libraryManager)
        {
        }
    }
}
