using Cratis.Chronicle.Concepts.Jobs;
using Moq;
namespace Cratis.Chronicle.Grains.Jobs.for_JobsManager;

public class when_resuming : given.the_manager
{
    JobId _jobId;
    Mock<INullJobWithSomeRequest> _job;

    void Establish()
    {
        _jobId = Guid.Parse("24ff9a76-a590-49b7-847d-28fcc9bf1024");
        _job = AddJob<INullJobWithSomeRequest>(_jobId);
    }

    Task Because() => _manager.Resume(_jobId);

    [Fact] void should_get_job_from_storage() => _jobStorage.Received(1).GetJob(_jobId);
    [Fact] void should_resume_the_job() => _job.Verify(_ => _.Resume(), Times.Once);
}