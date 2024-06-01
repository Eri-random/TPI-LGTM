import { TestBed } from '@angular/core/testing';

import { GenerateIdeaService } from './generate-idea.service';

describe('GenerateIdeaService', () => {
  let service: GenerateIdeaService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(GenerateIdeaService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
