﻿var issueDetailService = function ($http) {

    return {
        getComments: getComments,
        getIssueMembers: getIssueMembers
    }


    function getComments(issueId) {
        return $http.get('../api/issues/' + issueId+"/comments")
      .then(getCommentsCompleted)
      .catch(getCommentsFailed);

        function getCommentsCompleted(response) {
            // console.log("r");
            //console.log(response.data);
            return response.data;
        }

        function getCommentsFailed(error) {
            console.log("error");
            return error;
        }
    }

    function getIssueMembers(issueId) {
        return $http.get('../../issues/members/' + issueId)
      .then(getIssueMembersCompleted)
      .catch(getIssueMembersFailed);

        function getIssueMembersCompleted(response) {
            // console.log("r");
            //console.log(response.data);
            return response.data;
        }

        function getIssueMembersFailed(error) {
            console.log("error");
            return error;
        }
    }
    
}

issueDetailService.$inject = ['$http'];

angular.module("issueDetialApp").factory("issueDetailService", issueDetailService);