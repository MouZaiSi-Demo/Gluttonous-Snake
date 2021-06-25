USE [test]
GO
/****** Object:  Table [dbo].[t_user]    Script Date: 05/29/2021 14:48:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[t_user](
	[userName] [nchar](10) NOT NULL,
	[userID] [nchar](10) NOT NULL,
	[phone] [nchar](11) NULL,
	[address] [nchar](80) NULL
) ON [PRIMARY]
GO
INSERT [dbo].[t_user] ([userName], [userID], [phone], [address]) VALUES (N'wang      ', N'001       ', N'13022556984', N'浙江温州                                                                            ')
INSERT [dbo].[t_user] ([userName], [userID], [phone], [address]) VALUES (N'zhang     ', N'002       ', N'13562232589', N'浙江温州                                                                            ')
