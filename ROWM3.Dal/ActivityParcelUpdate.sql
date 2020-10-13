/****** Object:  Trigger [ROWM].[ActivityParcelUpdate]    Script Date: 10/13/2020 4:48:44 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TRIGGER [ROWM].[ActivityParcelUpdate] 
   ON  [ROWM].[StatusActivity] 
   AFTER INSERT
AS 
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for trigger here
	with touched as (
	select i.ParentParcelId, i.ParcelStatusCode, s.DisplayOrder
	from rowm.StatusActivity i
	inner join rowm.Parcel_Status s on s.Code = i.ParcelStatusCode 
	)

	update rowm.Parcel
	set ParcelStatusCode = t.ParcelStatusCode, LastModified = GETDATE(), ModifiedBy = 'ActivityParcelUpdate TRIGGER'
	from rowm.Parcel p
	inner join touched t on p.ParcelId = t.ParentParcelId
	inner join rowm.Parcel_Status s on p.ParcelStatusCode = s.code
	and t.DisplayOrder > s.DisplayOrder

END
GO

ALTER TABLE [ROWM].[StatusActivity] ENABLE TRIGGER [ActivityParcelUpdate]
GO


