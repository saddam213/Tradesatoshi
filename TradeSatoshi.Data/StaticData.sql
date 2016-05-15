SET IDENTITY_INSERT [dbo].[EmailTemplate] ON 

GO
INSERT [dbo].[EmailTemplate] ([Id], [Type], [Subject], [Template], [IsHtml], [IsEnabled], [From], [Description], [Parameters]) 
VALUES (1, 0, N'Logon', N'User: [USERNAME], IpAddress: [IPADDRESS], LockoutLink: [LOCKOUTLINK]', 1, 1, N'system@cryptopia.co.nz', N'Logon', N'[USERNAME] = The current user name || [IPADDRESS] = The current ipaddress of the user || [LOCKOUTLINK] = link to lock the users account')
GO
INSERT [dbo].[EmailTemplate] ([Id], [Type], [Subject], [Template], [IsHtml], [IsEnabled], [From], [Description], [Parameters]) 
VALUES (2, 1, N'FailedLogon', N'User: [USERNAME], IpAddress: [IPADDRESS], LockoutLink: [LOCKOUTLINK]', 1, 1, N'system@cryptopia.co.nz', N'FailedLogon', N'[USERNAME] = The current user name || [IPADDRESS] = The current ipaddress of the user || [LOCKOUTLINK] = link to lock the users account')
GO
INSERT [dbo].[EmailTemplate] ([Id], [Type], [Subject], [Template], [IsHtml], [IsEnabled], [From], [Description], [Parameters]) 
VALUES (3, 2, N'PasswordLockout', N'User: [USERNAME], IpAddress: [IPADDRESS]', 1, 1, N'system@cryptopia.co.nz', N'PasswordLockout', N'[USERNAME] = The current user name || [IPADDRESS] = The current ipaddress of the user')
GO
INSERT [dbo].[EmailTemplate] ([Id], [Type], [Subject], [Template], [IsHtml], [IsEnabled], [From], [Description], [Parameters]) 
VALUES (4, 3, N'UserLockout', N'User: [USERNAME], IpAddress: [IPADDRESS]', 1, 1, N'system@cryptopia.co.nz', N'UserLockout', N'[USERNAME] = The current user name || [IPADDRESS] = The current ipaddress of the user || [CONFIRMLINK] = Link to confirm the users email address')
GO
INSERT [dbo].[EmailTemplate] ([Id], [Type], [Subject], [Template], [IsHtml], [IsEnabled], [From], [Description], [Parameters]) 
VALUES (5, 4, N'Registration', N'User: [USERNAME], IpAddress: [IPADDRESS], ConfirmLink: [CONFIRMLINK]', 1, 1, N'system@cryptopia.co.nz', N'Registration', N'[USERNAME] = The current user name || [IPADDRESS] = The current ipaddress of the user')
GO
INSERT [dbo].[EmailTemplate] ([Id], [Type], [Subject], [Template], [IsHtml], [IsEnabled], [From], [Description], [Parameters]) 
VALUES (6, 5, N'TwoFactorLogin', N'User: [USERNAME], IpAddress: [IPADDRESS], TFACode: [TFACODE]', 1, 1, N'system@cryptopia.co.nz', N'TwoFactorLogin', N'[USERNAME] = The current user name || [IPADDRESS] = The current ipaddress of the user || [TFACODE] = The TFA code')
GO
INSERT [dbo].[EmailTemplate] ([Id], [Type], [Subject], [Template], [IsHtml], [IsEnabled], [From], [Description], [Parameters]) 
VALUES (7, 6, N'TwoFactorUnlockCode', N'User: [USERNAME], IpAddress: [IPADDRESS], TFACode: [TFACODE], TFAType: [TFATYPE]', 1, 1, N'system@cryptopia.co.nz', N'TwoFactorUnlockCode', N'[USERNAME] = The current user name || [IPADDRESS] = The current ipaddress of the user || [TFACODE] = The TFA code || [TFATYPE] = The type of twofactor e.g. Login, Withdraw, Transfer')
GO
INSERT [dbo].[EmailTemplate] ([Id], [Type], [Subject], [Template], [IsHtml], [IsEnabled], [From], [Description], [Parameters]) 
VALUES (8, 7, N'PasswordReset', N'User: [USERNAME], IpAddress: [IPADDRESS], ResetLink: [RESETLINK]', 1, 1, N'system@cryptopia.co.nz', N'PasswordReset', N'[USERNAME] = The current user name || [IPADDRESS] = The current ipaddress of the user || [RESETLINK] = The reset link')
GO
INSERT [dbo].[EmailTemplate] ([Id], [Type], [Subject], [Template], [IsHtml], [IsEnabled], [From], [Description], [Parameters]) 
VALUES (9, 8, N'TwoFactorWithdraw', N'User: [USERNAME], IpAddress: [IPADDRESS], TFACode: [TFACODE]', 1, 1, N'system@cryptopia.co.nz', N'TwoFactorWithdraw', N'[USERNAME] = The current user name || [IPADDRESS] = The current ipaddress of the user || [TFACODE] = The TFA code')
GO
INSERT [dbo].[EmailTemplate] ([Id], [Type], [Subject], [Template], [IsHtml], [IsEnabled], [From], [Description], [Parameters]) 
VALUES (10, 9, N'WithdrawConfirmation', N'User: [USERNAME], IpAddress: [IPADDRESS], ConfirmLink: [CONFIRMLINK], CancelLink: [CANCELLINK], WithdrawId: [WITHDRAWID], Currency: [Currency], Amount: [AMOUNT], Address: [ADDRESS], Fee: [FEE]', 1, 1, N'system@cryptopia.co.nz', N'WithdrawConfirmation', N'[USERNAME] = The current user name || [IPADDRESS] = The current ipaddress of the user || [CONFIRMLINK] = The link to confirm the withdrawal || [CANCELLINK] = the link to cancel the withdrawal || [WITHDRAWID] = The withdrawal id || [Currency] = The currency || [AMOUNT] = The amount || [ADDRESS] = The address || [FEE] = the fee.')
GO
SET IDENTITY_INSERT [dbo].[EmailTemplate] OFF