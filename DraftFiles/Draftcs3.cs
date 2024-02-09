


    //database update logic
    void UpdateDatabase()
    {
       using (var connection = Database.Open("smartcv"))
       {
        // SQL command to update the database table
        var recordId = Request["id"];

        var updateQuery = "UPDATE events SET eIsPinned = 1 WHERE ID = @0";
        db.Execute(updateQuery, recordId);

       }
    }

    // Check if the request is a POST request to the "/UpdateDatabase" endpoint
    if (IsPost && Request.Path.ToLower() == "/application-pane.cshtml")
    {
       UpdateDatabase();
    }

    var recordId = Request["id"];
    // Update the eIsPinned field in the database
    //var db = Database.Open("smartcv");
    var updateQuery = "UPDATE events SET eIsPinned = 1 WHERE ID = @0";
    db.Execute(updateQuery, recordId);
    db.Close();




@{
    string resultMessage = "";

    if (IsPost)
    {
        // Retrieve the value of eId from the POST request
        int eId = Convert.ToInt32(Request["eId"]);

        using (var db = Database.Open("smartcv"))
        {
            // SQL command to select the current value of eIsPinned
            var selectQuery = "SELECT eIsPinned FROM events WHERE eId = @0";
            var currentPinnedValue = db.QueryValue(selectQuery, eId);

            // Toggle the value of eIsPinned
            int newPinnedValue = (currentPinnedValue == DBNull.Value || (int)currentPinnedValue == 0) ? 1 : 0;

            // SQL command to update the database table
            var updateQuery = "UPDATE events SET eIsPinned = @0 WHERE eId = @1";
            db.Execute(updateQuery, newPinnedValue, eId);

            resultMessage = "Success";
        }
    }
}

@resultMessage






<div id="resultArea">
    @resultMessage
</div>

<script>
    function updatePinnedButton(eId) {
        // Perform AJAX POST request to update the database
        $.post("/data/update-Activity-Pinned", { eId: eId }, function (response) {
            // Update result message with the response
            $('#resultArea').html(response);
        })
            .fail(function () {
                // Handle error if AJAX request fails
                $('#resultArea').html('Error occurred while updating the database.');
            });

        // Prevent default link behavior
        return false;
    }
</script>

<div id="resultArea">
    @resultMessage
</div>

<script>
    function updatePinnedButton(eId) {
        // Perform AJAX request to update the database and load updated content into resultArea
        $('#resultArea').load("/data/update-Activity-Pinned", { eId: eId }, function (response, status, xhr) {
            if (status == "error") {
                // Handle error if AJAX request fails
                $('#resultArea').html('Error occurred while updating the database.');
            }
        });

        // Prevent default link behavior
        return false;
    }
</script>










// Program.cs (Main file)
using System;

class Program
{
    static void Main(string[] args)
    {
        MyClass myClass = new MyClass();
        myClass.Method1();
        myClass.Method2();
    }
}

// File1.cs
using System;

public partial class MyClass
{
    public void Method1()
    {
        Console.WriteLine("Method1 from File1.cs");
    }
}

// File2.cs
using System;

public partial class MyClass
{
    public void Method2()
    {
        Console.WriteLine("Method2 from File2.cs");
    }
}

















<div id="resultArea">
    @resultMessage
</div>

<form id="updateForm">
    <!-- Your form fields here -->
    <input type="hidden" id="eId" name="eId" value="@(eId)" />
    <button type="submit">Update</button>
</form>

<script>
    $(document).ready(function () {
        $('#updateForm').submit(function (event) {
            event.preventDefault(); // Prevent the default form submission

            var formData = $(this).serialize(); // Serialize form data

            $.ajax({
                type: 'POST',
                url: '@Url.Action("YourAction", "YourController")', // Replace YourAction and YourController with actual values
                data: formData,
                success: function (response) {
                    $('#resultArea').html(response); // Update the result area with the response from the server
                },
                error: function () {
                    alert('An error occurred while processing your request.');
                }
            });
        });
    });
</script>





<button class="clickable-button" onclick="updatePinnedButton(@timelineItem.eId)">
    <div style="background-color: #bbb; float: right; padding: 3px;">
        <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" fill="currentColor" class="bi bi-pin-angle-fill" viewBox="0 0 16 16">
            <path d="M9.828.722a.5.5 0 0 1 .354.146l4.95 4.95a.5.5 0 0 1 0 .707c-.48.48-1.072.588-1.503.588-.177 0-.335-.018-.46-.039l-3.134 3.134a6 6 0 0 1 .16 1.013c.046.702-.032 1.687-.72 2.375a.5.5 0 0 1-.707 0l-2.829-2.828-3.182 3.182c-.195.195-1.219.902-1.414.707s.512-1.22.707-1.414l3.182-3.182-2.828-2.829a.5.5 0 0 1 0-.707c.688-.688 1.673-.767 2.375-.72a6 6 0 0 1 1.013.16l3.134-3.133a3 3 0 0 1-.04-.461c0-.43.108-1.022.589-1.503a.5.5 0 0 1 .353-.146" />
        </svg>
    </div>
</button>




@{
    // Import necessary namespaces and classes
    using System;
    using System.Linq;

    // Check if the form is submitted and the request is a POST request
    if (Request.HttpMethod == "POST")
    {
        try
        {
            // Retrieve the value of eId from the POST request
            int eId = Convert.ToInt32(Request["eId"]);

            // Your code to update the database table 'events' and set 'eIsPinned' field
            // Assuming you have a method to update the database, you might use Entity Framework or similar ORM
            // Example using Entity Framework:
            using (var db = new YourDbContext())
            {
                var eventData = db.Events.FirstOrDefault(e => e.Id == eId);
                if (eventData != null)
                {
                    eventData.eIsPinned = true; // Update the 'eIsPinned' field to true
                    db.SaveChanges(); // Save changes to the database
                }
            }

            // Set the result message
            string resultMessage = "Update successful";
        }
        catch (Exception ex)
        {
            // Handle any exceptions and set the error message
            string resultMessage = "Error occurred while updating the database: " + ex.Message;
        }
    }
    else
    {
        // If the request is not a POST request, initialize the result message
        string resultMessage = "";
    }
}
