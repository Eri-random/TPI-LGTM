import { TestBed } from '@angular/core/testing';

import { ResponseIdeaService } from './response-idea.service';

describe('ResponseIdeaService', () => {
  let service: ResponseIdeaService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(ResponseIdeaService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
