/*
This file is the main entry point for defining Gulp tasks and using Gulp plugins.
Click here to learn more. https://go.microsoft.com/fwlink/?LinkId=518007
*/

'use strict';

const gulp = require('gulp');
var concat = require('gulp-concat');


gulp.task('merge-sql', function (done) {
	return gulp.src(['../Web Apps/PanacheSoftware.Identity/Data/SQL/IdentityUsers.sql',
		'../Web Apps/PanacheSoftware.Identity/Data/SQL/IdentityUserLogins.sql',
		'../Web Apps/PanacheSoftware.Identity/Data/SQL/IdentityUserClaims.sql',
		'../Web Apps/PanacheSoftware.Identity/Data/SQL/IdentityRoles.sql',
		'../Web Apps/PanacheSoftware.Identity/Data/SQL/IdentityRoleClaims.sql',
		'../Web Apps/PanacheSoftware.Identity/Data/SQL/IdentityUserRoles.sql',
		'../Web Apps/PanacheSoftware.Identity/Data/SQL/IdentityUserTokens.sql',
		'../Web Apps/PanacheSoftware.Identity/Data/SQL/IdentityTenants.sql',
		'../Services/PanacheSoftware.Service.Client/Persistance/SQL/ClientHeader.sql',
		'../Services/PanacheSoftware.Service.Client/Persistance/SQL/ClientDetail.sql',
		'../Services/PanacheSoftware.Service.Client/Persistance/SQL/ClientContact.sql',
		'../Services/PanacheSoftware.Service.Client/Persistance/SQL/ClientAddress.sql',
		'../Services/PanacheSoftware.Service.Team/Persistance/SQL/TeamHeader.sql',
		'../Services/PanacheSoftware.Service.Team/Persistance/SQL/TeamDetail.sql',
		'../Services/PanacheSoftware.Service.Team/Persistance/SQL/UserTeam.sql',
		'../Services/PanacheSoftware.Service.Foundation/Persistance/SQL/LanguageHeader.sql',
		'../Services/PanacheSoftware.Service.Foundation/Persistance/SQL/LanguageCode.sql',
		'../Services/PanacheSoftware.Service.Foundation/Persistance/SQL/LanguageItem.sql',
		'../Services/PanacheSoftware.Service.Foundation/Persistance/SQL/SettingHeader.sql',
		'../Services/PanacheSoftware.Service.Foundation/Persistance/SQL/UserSetting.sql',
		'../Services/PanacheSoftware.Service.Folder/Persistance/SQL/FolderHeader.sql',
		'../Services/PanacheSoftware.Service.Folder/Persistance/SQL/FolderDetail.sql',
		'../Services/PanacheSoftware.Service.Folder/Persistance/SQL/FolderNode.sql',
		'../Services/PanacheSoftware.Service.Folder/Persistance/SQL/FolderNodeDetail.sql',
		'../Services/PanacheSoftware.Service.Folder/Persistance/SQL/TeamFolder.sql',
		'../Services/PanacheSoftware.Service.Task/Persistance/SQL/TaskGroupHeader.sql',
		'../Services/PanacheSoftware.Service.Task/Persistance/SQL/TaskGroupDetail.sql',
		'../Services/PanacheSoftware.Service.Task/Persistance/SQL/TaskHeader.sql',
		'../Services/PanacheSoftware.Service.Task/Persistance/SQL/TaskDetail.sql',
		'../Services/PanacheSoftware.Service.Task/Persistance/SQL/TeamTask.sql',
		'../Services/PanacheSoftware.Service.File/Persistance/SQL/FileHeader.sql',
		'../Services/PanacheSoftware.Service.File/Persistance/SQL/FileDetail.sql',
		'../Services/PanacheSoftware.Service.File/Persistance/SQL/FileVersion.sql',
		'../Services/PanacheSoftware.Service.File/Persistance/SQL/FileLink.sql'])
		.pipe(concat('PanacheLegalTables.sql'))
		.pipe(gulp.dest('./SQL/'));
});

gulp.task('merge-drop-sql', function (done) {
	return gulp.src(['../Web Apps/PanacheSoftware.Identity/Data/SQL/DropTables.sql',
		'../Services/PanacheSoftware.Service.Client/Persistance/SQL/DropTables.sql',
		'../Services/PanacheSoftware.Service.Team/Persistance/SQL/DropTables.sql',
		'../Services/PanacheSoftware.Service.Foundation/Persistance/SQL/DropTables.sql',
		'../Services/PanacheSoftware.Service.Folder/Persistance/SQL/DropTables.sql',
		'../Services/PanacheSoftware.Service.Task/Persistance/SQL/DropTables.sql',
		'../Services/PanacheSoftware.Service.File/Persistance/SQL/DropTables.sql'])
		.pipe(concat('PanacheLegalDropTables.sql'))
		.pipe(gulp.dest('./SQL/'));
});

gulp.task('build', gulp.series('merge-sql', 'merge-drop-sql'));

gulp.task('default', gulp.series('build'));