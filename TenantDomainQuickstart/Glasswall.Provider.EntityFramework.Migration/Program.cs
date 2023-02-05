using System;
using System.Linq;
using System.Threading.Tasks;
using Glasswall.FileTrust.API.Dependencies;
using Glasswall.FileTrust.Domain;
using Glasswall.Kernel.Data.ORM;
using Glasswall.Kernel.Data.Tenancy;
using Glasswall.Kernel.DependencyResolver;
using Glasswall.Providers.Microsoft.DependencyInjection;

namespace Provider.EntityFramework.Migration
{
    class Program
    {
        private const string HubCacheKey = "Hub_Context";
        static void Main(string[] args)
        {
            var resolver = new MicrosoftDependencyInjection();
            Registration.Register(resolver);
            RegistrationForDb.Register(resolver);
            RegistrationForTenancy.Register(resolver);
            resolver.Initialise();
            
            var context = resolver.Resolve<IDbContext>();

            //var draftPolicy = Program.BuidPolicy(true);
            //Program.AssignTenantId(draftPolicy, resolver);
            //var currentPolicy = Program.BuidPolicy(false);
            //Program.AssignTenantId(currentPolicy, resolver);
            //context.Add(draftPolicy);
            //context.Add(currentPolicy);
            //context.SaveChanges();

            var policyCatalogues = context.Set<PolicyCatalogue>()
                .First(x => !x.IsDeleted && x.Published == null && x.Expired == null && x.Id == Guid.Parse("92f43bdf-a5c8-49d3-9d52-0cebb25effd0"));
            var copy = new PolicyCatalogue(policyCatalogues);
            var foo = context.Add(copy);
            context.SaveChanges();
            var boo = context.Set<PolicyCatalogue>()
                .First(x => x.Id == copy.Id);
            //Console.WriteLine("policy Catalogues: " + policyCatalogues.Count);
            Console.ReadKey();
        }

        private static Task AssignTenantId<T>(T item, IDependencyResolver resolver) where T : BaseTenantModel
        {
            var tenantContextAccessor = resolver.Resolve<IDbTenantContextAccessor>();
            var tenandId = tenantContextAccessor.DbTenantContext.TenantId;
            var tenantFilterBuilder = resolver.Resolve<ITenantFilterBuilder>();
            tenantFilterBuilder.AssignTenantId(item, tenandId);
            return Task.CompletedTask;
            
        }

        //private static PolicyCatalogue BuidPolicy(bool isDraft)
        //{
        //    var policyCatalogue = new PolicyCatalogue
        //    {
        //        AgentType = 0,
        //        Created = DateTimeOffset.Now,
        //        LastUpdated = DateTimeOffset.Now,
        //        LastUpdatedBy = "Admin"
        //    };
        //    if (!isDraft)
        //        policyCatalogue.Published = DateTime.Now;

        //    var senderGroup = new SenderGroup
        //    {
        //        Name = "All Senders"
        //    };

        //    var recieverGroup = new ReceiverGroup
        //    {
        //        Name = "All Receivers"
        //    };

        //    // create a threat censor set
        //    var threatCensorPolicySet = new ThreatCensorPolicy
        //    {
        //        Name = "Organisation ThreatCensor Policy Set"
        //    };
            
        //    var contentManagmentPolicyId = Guid.NewGuid();
        //    var contentManagmentPolicy = new ContentManagementPolicy
        //    {
        //        Id = contentManagmentPolicyId,
        //        Name = "Organisation Content Management Policy",
        //        DefaultMediaTypePolicy = 0,
        //        GlasswallPdfContentManagement = new GlasswallPdfContentManagement
        //        {
        //            Id = contentManagmentPolicyId,
        //            WaterMark = "Glasswall Protected",
        //            Javascript = 2,
        //            Metadata = 2,
        //            Acroform = 2,
        //            ActionsAll = 2,
        //            InternalHyperlinks = 2,
        //            ExternalHyperlinks = 2,
        //            EmbeddedFiles = 2
        //        },
        //        GlasswallExcelContentManagement = new GlasswallExcelContentManagement
        //        {
        //            Id = contentManagmentPolicyId,
        //            EmbeddedFiles = 2,
        //            ExternalHyperlinks = 2,
        //            InternalHyperlinks = 2,
        //            Macros = 2,
        //            Metadata = 2,
        //            ReviewComments = 2
        //        },

        //        GlasswallPowerPointContentManagement = new GlasswallPowerPointContentManagement
        //        {
        //            Id = contentManagmentPolicyId,
        //            EmbeddedFiles = 2,
        //            ExternalHyperlinks = 2,
        //            InternalHyperlinks = 2,
        //            Macros = 2,
        //            Metadata = 2,
        //            ReviewComments = 2
        //        },
        //        GlasswallWordContentManagement = new GlasswallWordContentManagement
        //        {
        //            Id = contentManagmentPolicyId,
        //            EmbeddedFiles = 2,
        //            ExternalHyperlinks = 2,
        //            InternalHyperlinks = 2,
        //            Macros = 2,
        //            Metadata = 2,
        //            ReviewComments = 2
        //        },
        //    };

        //    ////Add File Type Policies
        //    //contentManagmentPolicy.FileTypePolicy.Add(new FileTypePolicy { Action = 2, FileType = "doc" });
        //    //contentManagmentPolicy.FileTypePolicy.Add(new FileTypePolicy { Action = 2, FileType = "docx" });
        //    //contentManagmentPolicy.FileTypePolicy.Add(new FileTypePolicy { Action = 2, FileType = "xls" });
        //    //contentManagmentPolicy.FileTypePolicy.Add(new FileTypePolicy { Action = 2, FileType = "xlsx" });
        //    //contentManagmentPolicy.FileTypePolicy.Add(new FileTypePolicy { Action = 2, FileType = "ppt" });
        //    //contentManagmentPolicy.FileTypePolicy.Add(new FileTypePolicy { Action = 2, FileType = "pptx" });
        //    //contentManagmentPolicy.FileTypePolicy.Add(new FileTypePolicy { Action = 2, FileType = "pdf" });
        //    //contentManagmentPolicy.FileTypePolicy.Add(new FileTypePolicy { Action = 2, FileType = "gif" });
        //    //contentManagmentPolicy.FileTypePolicy.Add(new FileTypePolicy { Action = 2, FileType = "png" });
        //    //contentManagmentPolicy.FileTypePolicy.Add(new FileTypePolicy { Action = 2, FileType = "jpg" });
        //    //contentManagmentPolicy.FileTypePolicy.Add(new FileTypePolicy { Action = 2, FileType = "jpeg" });
        //    //contentManagmentPolicy.FileTypePolicy.Add(new FileTypePolicy { Action = 2, FileType = "jpe" });
        //    //contentManagmentPolicy.FileTypePolicy.Add(new FileTypePolicy { Action = 2, FileType = "zip" });
        //    //contentManagmentPolicy.FileTypePolicy.Add(new FileTypePolicy { Action = 2, FileType = "dot" });
        //    //contentManagmentPolicy.FileTypePolicy.Add(new FileTypePolicy { Action = 2, FileType = "dotx" });
        //    //contentManagmentPolicy.FileTypePolicy.Add(new FileTypePolicy { Action = 2, FileType = "docm" });
        //    //contentManagmentPolicy.FileTypePolicy.Add(new FileTypePolicy { Action = 2, FileType = "dotm" });
        //    //contentManagmentPolicy.FileTypePolicy.Add(new FileTypePolicy { Action = 2, FileType = "xlt" });
        //    //contentManagmentPolicy.FileTypePolicy.Add(new FileTypePolicy { Action = 2, FileType = "xlsm" });
        //    //contentManagmentPolicy.FileTypePolicy.Add(new FileTypePolicy { Action = 2, FileType = "xltx" });
        //    //contentManagmentPolicy.FileTypePolicy.Add(new FileTypePolicy { Action = 2, FileType = "xltm" });
        //    //contentManagmentPolicy.FileTypePolicy.Add(new FileTypePolicy { Action = 2, FileType = "pot" });
        //    //contentManagmentPolicy.FileTypePolicy.Add(new FileTypePolicy { Action = 2, FileType = "pps" });
        //    //contentManagmentPolicy.FileTypePolicy.Add(new FileTypePolicy { Action = 2, FileType = "potx" });
        //    //contentManagmentPolicy.FileTypePolicy.Add(new FileTypePolicy { Action = 2, FileType = "ppsx" });
        //    //contentManagmentPolicy.FileTypePolicy.Add(new FileTypePolicy { Action = 2, FileType = "potm" });
        //    //contentManagmentPolicy.FileTypePolicy.Add(new FileTypePolicy { Action = 2, FileType = "pptm" });
        //    //contentManagmentPolicy.FileTypePolicy.Add(new FileTypePolicy { Action = 2, FileType = "ppsm" });

        //    //Add Mime Type Policies
        //    //contentManagmentPolicy.MimeTypePolicy.Add(new MimeTypePolicy { Action = 2, MimeType = "multipart/encrypted" });
        //    //contentManagmentPolicy.MimeTypePolicy.Add(new MimeTypePolicy { Action = 2, MimeType = "multipart/signed" });
        //    //contentManagmentPolicy.MimeTypePolicy.Add(new MimeTypePolicy { Action = 2, MimeType = "application/x-zip-compressed" });
        //    //contentManagmentPolicy.MimeTypePolicy.Add(new MimeTypePolicy { Action = 2, MimeType = "application/zip" });
        //    //contentManagmentPolicy.MimeTypePolicy.Add(new MimeTypePolicy { Action = 2, MimeType = "image/jpeg" });
        //    //contentManagmentPolicy.MimeTypePolicy.Add(new MimeTypePolicy { Action = 2, MimeType = "image/png" });
        //    //contentManagmentPolicy.MimeTypePolicy.Add(new MimeTypePolicy { Action = 2, MimeType = "image/gif" });
        //    //contentManagmentPolicy.MimeTypePolicy.Add(new MimeTypePolicy { Action = 2, MimeType = "application/pdf" });
        //    //contentManagmentPolicy.MimeTypePolicy.Add(new MimeTypePolicy { Action = 2, MimeType = "application/msword" });
        //    //contentManagmentPolicy.MimeTypePolicy.Add(new MimeTypePolicy { Action = 2, MimeType = "application/vnd.openxmlformats-officedocument.wordprocessingml.document" });
        //    //contentManagmentPolicy.MimeTypePolicy.Add(new MimeTypePolicy { Action = 2, MimeType = "application/vnd.openxmlformats-officedocument.wordprocessingml.template" });
        //    //contentManagmentPolicy.MimeTypePolicy.Add(new MimeTypePolicy { Action = 2, MimeType = "application/vnd.ms-word.document.macroEnabled.12" });
        //    //contentManagmentPolicy.MimeTypePolicy.Add(new MimeTypePolicy { Action = 2, MimeType = "application/vnd.ms-word.template.macroEnabled.12" });
        //    //contentManagmentPolicy.MimeTypePolicy.Add(new MimeTypePolicy { Action = 2, MimeType = "application/vnd.ms-excel" });
        //    //contentManagmentPolicy.MimeTypePolicy.Add(new MimeTypePolicy { Action = 2, MimeType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" });
        //    //contentManagmentPolicy.MimeTypePolicy.Add(new MimeTypePolicy { Action = 2, MimeType = "application/vnd.openxmlformats-officedocument.spreadsheetml.template" });
        //    //contentManagmentPolicy.MimeTypePolicy.Add(new MimeTypePolicy { Action = 2, MimeType = "application/vnd.ms-excel.sheet.macroEnabled.12" });
        //    //contentManagmentPolicy.MimeTypePolicy.Add(new MimeTypePolicy { Action = 2, MimeType = "application/vnd.ms-excel.template.macroEnabled.12" });
        //    //contentManagmentPolicy.MimeTypePolicy.Add(new MimeTypePolicy { Action = 2, MimeType = "application/vnd.ms-powerpoint" });
        //    //contentManagmentPolicy.MimeTypePolicy.Add(new MimeTypePolicy { Action = 2, MimeType = "application/vnd.openxmlformats-officedocument.presentationml.presentation" });
        //    //contentManagmentPolicy.MimeTypePolicy.Add(new MimeTypePolicy { Action = 2, MimeType = "application/vnd.openxmlformats-officedocument.presentationml.template" });
        //    //contentManagmentPolicy.MimeTypePolicy.Add(new MimeTypePolicy { Action = 2, MimeType = "application/vnd.openxmlformats-officedocument.presentationml.slideshow" });
        //    //contentManagmentPolicy.MimeTypePolicy.Add(new MimeTypePolicy { Action = 2, MimeType = "application/vnd.ms-powerpoint.presentation.macroEnabled.12" });
        //    //contentManagmentPolicy.MimeTypePolicy.Add(new MimeTypePolicy { Action = 2, MimeType = "application/vnd.ms-powerpoint.template.macroEnabled.12" });
        //    //contentManagmentPolicy.MimeTypePolicy.Add(new MimeTypePolicy { Action = 2, MimeType = "application/vnd.ms-powerpoint.slideshow.macroEnabled.12" });

        //    //Add members to policy catalogue
        //    policyCatalogue.ContentManagementPolicy.Add(contentManagmentPolicy);
        //    policyCatalogue.ReceiverGroup.Add(recieverGroup);
        //    policyCatalogue.SenderGroup.Add(senderGroup);
        //    policyCatalogue.ThreatCensorPolicy.Add(threatCensorPolicySet);

        //    //create a processing rule with existing menmbers in policy catalogue
        //    //var processingRule = new ProcessingRule
        //    //{
        //    //    Id = Guid.NewGuid(),
        //    //    Priority = 0,
        //    //    ReceiverGroup = recieverGroup,
        //    //    SenderGroup = senderGroup,
        //    //    ThreatCensorPolicySet = threatCensorPolicySet,
        //    //    ContentManagementPolicy = contentManagmentPolicy
        //    //};

        //    //policyCatalogue.ProcessingRule.Add(processingRule);
        //    return policyCatalogue;
        //}
    }
}
