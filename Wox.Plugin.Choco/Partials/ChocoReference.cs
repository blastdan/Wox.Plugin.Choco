using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Wox.Plugin.Choco.ChocoReference
{
    [global::System.Data.Services.Common.EntitySetAttribute("Packages")]
    [global::System.Data.Services.Common.EntityPropertyMappingAttribute("Id", global::System.Data.Services.Common.SyndicationItemProperty.Title, global::System.Data.Services.Common.SyndicationTextContentKind.Plaintext, false)]
    [global::System.Data.Services.Common.EntityPropertyMappingAttribute("Authors", global::System.Data.Services.Common.SyndicationItemProperty.AuthorName, global::System.Data.Services.Common.SyndicationTextContentKind.Plaintext, false)]
    [global::System.Data.Services.Common.EntityPropertyMappingAttribute("LastUpdated", global::System.Data.Services.Common.SyndicationItemProperty.Updated, global::System.Data.Services.Common.SyndicationTextContentKind.Plaintext, false)]
    [global::System.Data.Services.Common.EntityPropertyMappingAttribute("Summary", global::System.Data.Services.Common.SyndicationItemProperty.Summary, global::System.Data.Services.Common.SyndicationTextContentKind.Plaintext, false)]
    [global::System.Data.Services.Common.HasStreamAttribute()]
    public partial class V2FeedPackage
    {
    }
}
