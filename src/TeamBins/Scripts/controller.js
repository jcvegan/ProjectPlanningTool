﻿var IssueDetails = IssueDetails || {};
IssueDetails.gIssueDetailConnectionID = "";

//
var issueDetailApp = angular.module('issueDetialApp', ['ngSanitize']);

var IssueDetailsCtrl = function($scope, $http, issueDetailService, issue) {

    var vm = this;

    vm.newComment = "";

    vm.hover = false;
    vm.members = [];
    vm.issue = issue;

    issueDetailService.getComments(vm.issue.id)
        .then(function(data) {
            vm.comments = data;
        });

    issueDetailService.getIssueMembers(vm.issue.id)
        .then(function (data) {
            vm.members = data;
            vm.isEditableForCurrentUser = true;
        });

    vm.saveComment = function () {
       
        if (vm.newComment != "") {
            issueDetailService.saveComment(vm.newComment, vm.issue.id, IssueDetails.gIssueDetailConnectionID)
                .then(function(response) {
                    if (response.Status === "Success") {
                        vm.newComment = "";
                        vm.comments.push(response.Data);
                    } else {
                        alert("Failed!");
                    }
                });
        }
    }
    

    vm.hover = function(member) {
        return member.ShowDelete = !member.ShowDelete;
    };
    vm.removeIssueMember = function(issueId, member, $event) {
        $http.post("../../issues/removemember", { memberid: member.MemberID, id: issueId }).success(function(data) {
            if (data.Status === "Success") {
                vm.members.splice(vm.members.indexOf(member), 1);
            }
        });
    };
    vm.removeComment = function(comment, issueId, $event) {
        var yes = window.confirm("Are you sure to delete this comment ? This cannot be undone.");
        if (yes) {
            $http.post("../../api/comment/" + comment.ID+"/delete").success(function (data) {
                if (data.Status === "Success") {
                    vm.comments.splice(vm.comments.indexOf(comment), 1);
                }
            });
        }
    };
    var chat = $.connection.issuesHub;
    chat.client.addNewComment = function (comment) {
       
        vm.comments.push(comment);

        vm.$apply();
    };
    $.connection.hub.start().done(function() {
        chat.server.subscribeToTeam($("#TeamID").val())
        IssueDetails.gIssueDetailConnectionID = $.connection.hub.id;
    });

};

IssueDetailsCtrl.$inject = ['$scope', '$http', 'issueDetailService', 'issue'];

issueDetailApp.controller("IssueDetailsCtrl", IssueDetailsCtrl);