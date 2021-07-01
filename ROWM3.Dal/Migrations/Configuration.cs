namespace ROWM.Dal.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<ROWM.Dal.ROWM_Context>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
            ContextKey = "ROWM.Dal.ROWM_Context";
        }

        protected override void Seed(ROWM.Dal.ROWM_Context context)
        {
            context.Contact_Purpose.AddOrUpdate(
                r => r.PurposeCode,
                new Contact_Purpose { PurposeCode = "ROE", Description = "ROE", DisplayOrder = 1, IsActive = true },
                new Contact_Purpose { PurposeCode = "Offer", Description = "Offer", DisplayOrder = 2, IsActive = true },
                new Contact_Purpose { PurposeCode = "Negotiation", Description = "Negotiation", DisplayOrder = 3, IsActive = true }
            );

            context.Repesentation_Type.AddOrUpdate(
                r => r.RelationTypeCode,
                new Repesentation_Type { RelationTypeCode = "Self", Description = "Self", DisplayOrder = 1, IsActive = true },
                new Repesentation_Type { RelationTypeCode = "Spouse", Description = "Spouse", DisplayOrder = 2, IsActive = true },
                new Repesentation_Type { RelationTypeCode = "Child", Description = "Child", DisplayOrder = 3, IsActive = true },
                new Repesentation_Type { RelationTypeCode = "Tenant", Description = "Tenant", DisplayOrder = 4, IsActive = true },
                new Repesentation_Type { RelationTypeCode = "Attorney", Description = "Attorney", DisplayOrder = 5, IsActive = true },
                new Repesentation_Type { RelationTypeCode = "Other", Description = "Other", DisplayOrder = 6, IsActive = true }
            );

            context.Contact_Channel.AddOrUpdate(
                c => c.ContactTypeCode,
                new Contact_Channel { ContactTypeCode = "Email", Description = "Email", DisplayOrder = 1, IsActive = true },
                new Contact_Channel { ContactTypeCode = "In-Person", Description = "In-Person", DisplayOrder = 2, IsActive = true },
                new Contact_Channel { ContactTypeCode = "Letter", Description = "Letter", DisplayOrder = 3, IsActive = true },
                new Contact_Channel { ContactTypeCode = "Note to File", Description = "Note to File", DisplayOrder = 4, IsActive = true },
                new Contact_Channel { ContactTypeCode = "Phone Call", Description = "Phone Call", DisplayOrder = 5, IsActive = true },
                new Contact_Channel { ContactTypeCode = "Text Message", Description = "Text Message", DisplayOrder = 6, IsActive = true }
            );

            context.Parcel_Status.AddOrUpdate(
                c => c.Code,
                new Parcel_Status { Code = "No_Activity", Description = "No Activity", DomainValue = 0, DisplayOrder = 0, IsActive = true },
                new Parcel_Status { Code = "Owner_Contacted", Description = "Owner Contacted", DomainValue = 1, DisplayOrder = 1, IsActive = true },
                new Parcel_Status { Code = "ROE_Obtained", Description = "ROE Obtained", DomainValue = 2, DisplayOrder = 2, IsActive = true },
                new Parcel_Status { Code = "Offer_Made", Description = "Offer Made", DomainValue = 3, DisplayOrder = 3, IsActive = true },
                new Parcel_Status { Code = "Easement_Signed", Description = "Easement Signed", DomainValue = 4, DisplayOrder = 4, IsActive = true },
                new Parcel_Status { Code = "Compensation_Check_Cut", Description = "Compensation Check Cut", DomainValue = 5, DisplayOrder = 5, IsActive = true },
                new Parcel_Status { Code = "Documents_Recorded", Description = "Documents Recorded", DomainValue = 6, DisplayOrder = 6, IsActive = true },
                new Parcel_Status { Code = "Compensation_Received_by_Owner", Description = "Compensation Received by Owner", DomainValue = 7, DisplayOrder = 7, IsActive = true }
            );

            context.Roe_Status.AddOrUpdate(
                c => c.Code,
                new Roe_Status { Code = "No_Activity", Description = "No Activity", DomainValue = 0, DisplayOrder = 0, IsActive = true },
                new Roe_Status { Code = "ROE_In_Progress", Description = "ROE In Progress", DomainValue = 1, DisplayOrder = 1, IsActive = true },
                new Roe_Status { Code = "ROE_Obtained", Description = "ROE Obtained", DomainValue = 2, DisplayOrder = 2, IsActive = true },
                new Roe_Status { Code = "ROE_with_Conditions", Description = "ROE With Conditions", DomainValue = 3, DisplayOrder = 3, IsActive = true },
                new Roe_Status { Code = "No_Access", Description = "No Access", DomainValue = 4, DisplayOrder = 4, IsActive = true }
            );

            context.Landowner_Score.AddOrUpdate(
                c => c.Score,
                new Landowner_Score {  Score=0, DomainValue =0, Caption="Not Set", DisplayOrder = 0, IsActive=false},
                new Landowner_Score { Score = 1, DomainValue = 1, Caption = "No Activity", DisplayOrder = 1, IsActive = false },
                new Landowner_Score { Score = 2, DomainValue = 2, Caption = "Low", DisplayOrder = 2, IsActive = true },
                new Landowner_Score { Score = 3, DomainValue = 3, Caption = "Median", DisplayOrder = 3, IsActive = true },
                new Landowner_Score { Score = 4, DomainValue = 4, Caption = "Median High", DisplayOrder = 4, IsActive = true },
                new Landowner_Score { Score = 5, DomainValue = 5, Caption = "High", DisplayOrder = 5, IsActive = true }
                );

            context.Agent.AddOrUpdate(
                a => a.AgentName,
                new Agent
                {
                    AgentName = "DEFAULT",
                    IsActive = false,
                    Created = DateTimeOffset.Now
                });
        }
    }
}
