$(document).ready(function () {
    $('#yourButtonId').click(function () {
        $.ajax({
            type: 'POST',
            url: 'YourPage.aspx/UpdateDatabase', // or the URL of your Web API endpoint
            contentType: 'application/json; charset=utf-8',
            dataType: 'json',
            success: function (data) {
                // Handle success
                console.log('Database updated successfully');
            },
            error: function (error) {
                // Handle error
                console.log('Error updating database');
            }
        });
    });
});


[System.Web.Services.WebMethod]
public static void UpdateDatabase()
{
    // Perform database update logic here
}