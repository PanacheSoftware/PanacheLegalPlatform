/*
This file is the main entry point for defining Gulp tasks and using Gulp plugins.
Click here to learn more. https://go.microsoft.com/fwlink/?LinkId=518007
*/

'use strict';

const gulp = require('gulp');
const sass = require('gulp-sass');
const autoprefixer = require('gulp-autoprefixer');

sass.compiler = require('node-sass');

gulp.task('copy-bootstrap', function (done) {
	gulp.src('./node_modules/bootstrap/dist/css/bootstrap.css')
		.pipe(gulp.dest('./wwwroot/css'));
	gulp.src('./node_modules/bootstrap/dist/js/bootstrap.js')
		.pipe(gulp.dest('./wwwroot/js'));
	done();
});

gulp.task('copy-bootstrap-flatly', function (done) {
	gulp.src('./node_modules/bootswatch/dist/flatly/bootstrap.css')
		.pipe(gulp.dest('./wwwroot/css/bootstrap'));
	gulp.src('./node_modules/bootstrap/dist/js/bootstrap.js')
		.pipe(gulp.dest('./wwwroot/js'));
	done();
});

gulp.task('copy-chartjs', function (done) {
	gulp.src('./node_modules/chart.js/dist/Chart.css')
		.pipe(gulp.dest('./wwwroot/css/chart'));
	gulp.src('./node_modules/chart.js/dist/Chart.js')
		.pipe(gulp.dest('./wwwroot/js/chart'));
	done();
});

gulp.task('copy-d3', function (done) {
	gulp.src('./node_modules/d3/dist/d3.js')
		.pipe(gulp.dest('./wwwroot/js/d3'));
	done();
});

gulp.task('copy-datepicker', function (done) {
	gulp.src('./node_modules/bootstrap-datepicker/dist/css/bootstrap-datepicker3.css')
		.pipe(gulp.dest('./wwwroot/css/datepicker'));
	gulp.src('./node_modules/bootstrap-datepicker/dist/js/bootstrap-datepicker.js')
		.pipe(gulp.dest('./wwwroot/js/datepicker'));
	done();
});

gulp.task('copy-datatables', function (done) {
	gulp.src('./node_modules/datatables.net-bs4/css/dataTables.bootstrap4.min.css')
		.pipe(gulp.dest('./wwwroot/css/datatables'));
	gulp.src('./node_modules/datatables.net/js/jquery.dataTables.min.js')
		.pipe(gulp.dest('./wwwroot/js/datatables'));
	gulp.src('./node_modules/datatables.net-bs4/js/dataTables.bootstrap4.min.js')
		.pipe(gulp.dest('./wwwroot/js/datatables'));
	done();
});

gulp.task('copy-customscrollbar', function (done) {
	gulp.src('./node_modules/malihu-custom-scrollbar-plugin/jquery.mCustomScrollbar.css')
		.pipe(gulp.dest('./wwwroot/css/sidebar'));
	gulp.src('./node_modules/malihu-custom-scrollbar-plugin/jquery.mCustomScrollbar.concat.min.js')
		.pipe(gulp.dest('./wwwroot/js/sidebar'));
	done();
});

gulp.task('copy-gantt', function (done) {
	gulp.src('./node_modules/dhtmlx-gantt/codebase/dhtmlxgantt.css')
		.pipe(gulp.dest('./wwwroot/css/dhtmlx-gantt'));
	gulp.src('./node_modules/dhtmlx-gantt/codebase/dhtmlxgantt.js')
		.pipe(gulp.dest('./wwwroot/js/dhtmlx-gantt'));
	done();
});

gulp.task('copy-jquery', function (done) {
	gulp.src('./node_modules/jquery/dist/jquery.js')
		.pipe(gulp.dest('./wwwroot/js'));
	done();
});

gulp.task('copy-popper', function (done) {
	gulp.src('./node_modules/popper.js/dist/umd/popper.min.js')
		.pipe(gulp.dest('./wwwroot/js'));
	done();
});

gulp.task('copy-orgchart', function (done) {
	gulp.src('./node_modules/orgchart/dist/css/jquery.orgchart.min.css')
		.pipe(gulp.dest('./wwwroot/css/orgchart'));
	gulp.src('./node_modules/orgchart/dist/js/jquery.orgchart.min.js')
		.pipe(gulp.dest('./wwwroot/js/orgchart'));
	done();
});

gulp.task('copy-fontawesome', function (done) {
	gulp.src('./node_modules/@fortawesome/fontawesome-free/css/all.css')
		.pipe(gulp.dest('./wwwroot/css/fontawesome'));
	gulp.src('./node_modules/@fortawesome/fontawesome-free/webfonts/**/*')
		.pipe(gulp.dest('./wwwroot/css/webfonts'));
	done();
});

gulp.task('sass', function () {
	return gulp.src(['./wwwroot/sass/**/*.scss'])
		.pipe(sass().on('error', sass.logError))
		.pipe(autoprefixer({
			browsers: ['last 2 versions'],
			cascade: false
		}))
		.pipe(gulp.dest('./wwwroot/css'));
	//done();
});

gulp.task('build', gulp.series('copy-bootstrap-flatly', 'copy-chartjs', 'copy-d3', 'copy-datepicker', 'copy-datatables', 'copy-customscrollbar', 'copy-gantt', 'copy-jquery', 'copy-popper', 'copy-orgchart', 'copy-fontawesome', 'sass'));

gulp.task('default', gulp.series('build'));