google.charts.load('current', { packages: ['orgchart'] });

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