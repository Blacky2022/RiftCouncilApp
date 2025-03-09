using Microsoft.AspNetCore.Components;

namespace RiftCouncilAppUI.Pages
{
    public partial class Details
    {
        [Parameter]
        public string Id { get; set; }

        private SuggestionModel suggestion;
        private UserModel loggedInUser;

        private List<StatusModel> statuses;
        private string settingStatus = "";
        private string urlText = "";

        protected async override Task OnInitializedAsync()
        {
            suggestion = await suggestionData.GetSuggestion(Id);
            loggedInUser = await authProvider.GetUserFromAuth(userData);
            statuses = await statusData.GetAllStatuses();
        }

        private async Task CompleteSetStatus()
        {
            switch (settingStatus)
            {
                case "completed":
                    if (string.IsNullOrWhiteSpace(urlText))
                    {
                        return;
                    }
                    suggestion.SuggestionStatus = statuses.Where(s => s.StatusName.ToLower() == settingStatus.ToLower()).First();
                    suggestion.OwnerNotes = $"You are right, this is an important topic in our community. Fix is coming in next patch. We created a resource about it here <a href='{urlText}' target='_blank'>{urlText}</a>";
                    break;
                case "watching":
                    suggestion.SuggestionStatus = statuses.Where(s => s.StatusName.ToLower() == settingStatus.ToLower()).First();
                    suggestion.OwnerNotes = "We noticed the interest this suggestion is getting! If more people are interested we may follow it in next patches.";
                    break;
                case "upcoming":
                    suggestion.SuggestionStatus = statuses.Where(s => s.StatusName.ToLower() == settingStatus.ToLower()).First();
                    suggestion.OwnerNotes = "Great suggestion! This topic is currently in development :)";
                    break;
                case "dismissed":
                    suggestion.SuggestionStatus = statuses.Where(s => s.StatusName.ToLower() == settingStatus.ToLower()).First();
                    suggestion.OwnerNotes = "Sometimes a good idea doesn't fit within our scope and vision. This is one those ideas.";
                    break;
                default:
                    return;
            }
            settingStatus = null;
            await suggestionData.UpdateSuggestion(suggestion);
        }

        private void ClosePage()
        {
            navManager.NavigateTo("/");
        }

        private string GetUpVoteTopText()
        {
            if (suggestion.UserVotes.Count > 0)
            {
                return suggestion.UserVotes.Count.ToString("00");
            }
            else
            {
                if (suggestion.Author.Id == loggedInUser?.Id)
                {
                    return "Awaiting";
                }
                else
                {
                    return "Click to";
                }
            }
        }

        private string GetUpVoteBottomText()
        {
            if (suggestion.UserVotes?.Count > 1)
            {
                return "Upvotes";
            }
            else
            {
                return "Upvote";
            }
        }

        private async Task VoteUp()
        {
            if (loggedInUser is not null)
            {
                if (suggestion.Author.Id == loggedInUser.Id)
                {
                    return;
                }

                if (suggestion.UserVotes.Add(loggedInUser.Id) == false)
                {
                    suggestion.UserVotes.Remove(loggedInUser.Id);
                }

                await suggestionData.UpVoteSuggestion(suggestion.Id, loggedInUser.Id);
            }
            else
            {
                navManager.NavigateTo("/MicrosoftIdentity/Account/SignIn", true);
            }
        }

        private string GetVoteClass()
        {
            if (suggestion.UserVotes is null || suggestion.UserVotes.Count == 0)
            {
                return "suggestion-detail-entry-no-votes";
            }
            else if (suggestion.UserVotes.Contains(loggedInUser?.Id))
            {
                return "suggestion-detail-entry-voted";
            }
            else
            {
                return "suggestion-detail-entry-not-voted";
            }
        }

        private string GetStatusClass()
        {
            if (suggestion is null || suggestion.SuggestionStatus is null)
            {
                return "suggestion-detail-entry-status-none";
            }

            string output = suggestion.SuggestionStatus.StatusName switch
            {
                "Completed" => "suggestion-detail-entry-status-completed",
                "Watching" => "suggestion-detail-entry-status-watching",
                "Upcoming" => "suggestion-detail-entry-status-upcoming",
                "Dismissed" => "suggestion-detail-entry-status-dismissed",
                _ => "suggestion-detail-entry-status-none",
            };

            return output;
        }
    }
}