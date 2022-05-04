google.charts.load('current', { packages: ['orgchart', 'gantt'] });

window.createOrgChart = (params) => {
    var nodeIds = params.nodeIds;
    var nodeNames = params.nodeNames;
    var nodeParents = params.nodeParents;
    var currentNode = params.currentNode;
    var data = new google.visualization.DataTable();
    data.addColumn('string', 'Name');
    data.addColumn('string', 'Manager');
    data.addColumn('string', 'ToolTip');

    for (var i = 0; i < nodeIds.length; i++) {
        var parentId = nodeParents[i]
        if (nodeParents[i] == '00000000-0000-0000-0000-000000000000') {
            parentId = '';
        }

        var currentNodeName = nodeNames[i];
        if (nodeIds[i] == currentNode) {
            currentNodeName = nodeNames[i] + '<div style="color:red; font-style:italic">Current</div>';
        }

        data.addRow([{ 'v': nodeIds[i], 'f': currentNodeName }, parentId, nodeNames[i]]);
    }

    // Create the chart.
    var chart = new google.visualization.OrgChart(document.getElementById('chart_div'));
    // Draw the chart, setting the allowHtml option to true for the tooltips.
    chart.draw(data, { allowHtml: true, allowCollapse: true });
};

window.createGanttChart = (params) => {
    var data = new google.visualization.DataTable(params.ganttJSON);
    //var data = new google.visualization.DataTable();
    //data.addColumn('string', 'Task ID');
    //data.addColumn('string', 'Task Name');
    //data.addColumn('string', 'Resource')
    //data.addColumn('date', 'Start Date');
    //data.addColumn('date', 'End Date');
    //data.addColumn('number', 'Duration');
    //data.addColumn('number', 'Percent Complete');
    //data.addColumn('string', 'Dependencies');

    //data.addRow(['08d92045-9da0-446e-83e2-a65b49f7d097', 'First Task', '08d92045-9da0-446e-83e2-a65b49f7d097', new Date(2021, 5, 26), new Date(2021, 6, 24), null, 0, null]);
    //data.addRow(['08d92045-9da0-446e-83e2-a65b49f7d0XX', 'Find sources', '08d92045-9da0-446e-83e2-a65b49f7d097', new Date(2021, 6, 1), new Date(2021, 6, 10), null, 100, null]);

    //data.addRow(['Research', 'Find sources', new Date(2015, 0, 1), new Date(2015, 0, 5), null, 100, null]);
    //data.addRow(['Write', 'Write paper', new Date(2015, 0, 6), new Date(2015, 0, 9), null, 25, 'Research,Outline']);
    //data.addRow(['Cite', 'Create bibliography', new Date(2015, 0, 6), new Date(2015, 0, 7), null, 20, 'Research']);
    //data.addRow(['Complete', 'Hand in paper', new Date(2015, 0, 9), new Date(2015, 0, 10), null, 0, 'Cite,Write']);
    //data.addRow(['Outline', 'Outline paper', new Date(2015, 0, 5), new Date(2015, 0, 6), null, 100, 'Research']);
    //data.addRow(['Research2', 'Find sources2', new Date(2015, 0, 1), new Date(2015, 0, 5), null, 100, null]);
    //data.addRow(['Write2', 'Write paper2', new Date(2015, 0, 6), new Date(2015, 0, 9), null, 25, 'Research2,Outline2']);
    //data.addRow(['Cite2', 'Create bibliography2', new Date(2015, 0, 6), new Date(2015, 0, 7), null, 20, 'Research2']);
    //data.addRow(['Complete2', 'Hand in paper2', new Date(2015, 0, 9), new Date(2015, 0, 10), null, 0, 'Cite2,Write2']);
    //data.addRow(['Outline2', 'Outline paper2', new Date(2015, 0, 5), new Date(2015, 0, 6), null, 100, 'Research2']);
    //data.addRow(['Research3', 'Find sources3', new Date(2015, 0, 1), new Date(2015, 0, 5), null, 100, null]);
    //data.addRow(['Write3', 'Write paper3', new Date(2015, 0, 6), new Date(2015, 0, 9), null, 25, 'Research3,Outline3']);
    //data.addRow(['Cite3', 'Create bibliography3', new Date(2015, 0, 6), new Date(2015, 0, 7), null, 20, 'Research3']);
    //data.addRow(['Complete3', 'Hand in paper3', new Date(2015, 0, 9), new Date(2015, 0, 10), null, 0, 'Cite3,Write3']);
    //data.addRow(['Outline3', 'Outline paper3', new Date(2015, 0, 5), new Date(2015, 0, 6), null, 100, 'Research3']);

    var trackHeight = 40;
    //height: data.getNumberOfRows() * trackHeight,

    var options = {
        height: (data.getNumberOfRows() + 2) * trackHeight,
        gantt: {
            criticalPathEnabled: false,
            trackHeight: trackHeight,
            labelStyle: {
                fontName: 'Open Sans',
                fontSize: 14,
                color: 'white'
            }
        }
    };

    var chart = new google.visualization.Gantt(document.getElementById('chart_div'));

    chart.draw(data, options);
};